import {
    StyledAccess,
    StyledAccessUser,
    StyledOpenSharingPanel,
    StyledProperties,
    StyledSubtitle,
    StyledThumbnail,
    StyledTitle,
} from "./styles/styles.js";
import RectangleLoader from "@appserver/common/components/Loaders/RectangleLoader";
import { FileType } from "@appserver/common/constants";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import Tooltip from "@appserver/components/tooltip";
import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

const SingleItem = (props) => {
    const {
        t,
        selectedItem,
        isRecycleBinFolder,
        onSelectItem,
        setSharingPanelVisible,
        getFolderInfo,
        getIcon,
        getFolderIcon,
        getShareUsers,
    } = props;

    const [item, setItem] = useState({
        id: "",
        isFolder: false,
        title: "",
        iconUrl: "",
        thumbnailUrl: "",
        properties: [],
        access: {
            owner: {
                img: "",
                link: "",
            },
            others: [],
        },
    });

    const [isShowAllAccessUsers, setIsShowAllAccessUsers] = useState(false);

    const updateItemsInfo = async (selectedItem) => {
        const getItemIcon = (item, size) => {
            const extension = item.fileExst;
            const iconUrl = extension
                ? getIcon(size, extension)
                : getFolderIcon(item.providerKey, size);

            return iconUrl;
        };

        const getSingleItemProperties = (item) => {
            const styledLink = (text, href) => (
                <Link className="property-content" href={href} isHovered={true}>
                    {text}
                </Link>
            );

            const styledText = (text) => (
                <Text className="property-content">{text}</Text>
            );

            const parseAndFormatDate = (date) => {
                date = new Date(date);

                const normalize = (num) => {
                    if (num > 9) return num;
                    return `0${num}`;
                };

                let day = normalize(date.getDate()),
                    month = normalize(date.getMonth()),
                    year = date.getFullYear(),
                    hours = date.getHours(),
                    minutes = normalize(date.getMinutes()),
                    a_p = hours > 12 ? "AM" : "PM";

                if (hours === 0) hours = 12;
                else if (hours > 12) hours = hours - 12;

                return `${day}.${month}.${year} ${hours}:${minutes} ${a_p}`;
            };

            const getItemType = (fileType) => {
                switch (fileType) {
                    case FileType.Unknown:
                        return t("Common:Unknown");
                    case FileType.Archive:
                        return t("Common:Archive");
                    case FileType.Video:
                        return t("Common:Video");
                    case FileType.Audio:
                        return t("Common:Audio");
                    case FileType.Image:
                        return t("Common:Image");
                    case FileType.Spreadsheet:
                        return t("Home:Spreadsheet");
                    case FileType.Presentation:
                        return t("Home:Presentation");
                    case FileType.Document:
                        return t("Home:Document");
                    default:
                        return t("Home:Folder");
                }
            };

            const itemSize = item.isFolder
                ? `${item.foldersCount} ${t("Translations:Folders")} | ${
                      item.filesCount
                  } ${t("Files")}`
                : item.contentLength;

            const itemType = getItemType(item.fileType);

            let result = [
                {
                    title: t("Common:Owner"),
                    content: styledLink(
                        item.createdBy.displayName,
                        item.createdBy.profileUrl
                    ),
                },
                {
                    title: t("InfoPanel:Location"),
                    content: <RectangleLoader width="150" height="19" />,
                },
                {
                    title: t("Common:Type"),
                    content: styledText(itemType),
                },
                {
                    title: t("Common:Size"),
                    content: styledText(itemSize),
                },
                {
                    title: t("Home:ByLastModifiedDate"),
                    content: styledText(parseAndFormatDate(item.updated)),
                },
                {
                    title: t("LastModifiedBy"),
                    content: styledLink(
                        item.updatedBy.displayName,
                        item.updatedBy.profileUrl
                    ),
                },
                {
                    title: t("Home:ByCreationDate"),
                    content: styledText(parseAndFormatDate(item.created)),
                },
            ];

            if (item.isFolder) return result;

            result.splice(3, 0, {
                title: t("FileExtension"),
                content: styledText(item.fileExst.split(".")[1].toUpperCase()),
            });

            result.push(
                {
                    title: t("Versions"),
                    content: styledText(item.version),
                },
                {
                    title: t("Comments"),
                    content: styledText(item.comment),
                }
            );

            return result;
        };

        const displayedItem = {
            id: selectedItem.id,
            isFolder: selectedItem.isFolder,
            title: selectedItem.title,
            iconUrl: getItemIcon(selectedItem, 32),
            thumbnailUrl:
                selectedItem.thumbnailUrl || getItemIcon(selectedItem, 96),
            properties: getSingleItemProperties(selectedItem),
            access: {
                owner: {
                    img: selectedItem.createdBy.avatarSmall,
                    link: selectedItem.createdBy.profileUrl,
                },
                others: [],
            },
        };

        setItem(displayedItem);
        loadAsyncData(displayedItem, selectedItem);
    };

    const loadAsyncData = async (displayedItem, selectedItem) => {
        const updateLoadedItemProperties = async (
            displayedItem,
            selectedItem
        ) => {
            const parentFolderId = selectedItem.isFolder
                ? selectedItem.parentId
                : selectedItem.folderId;
            const folderInfo = await getFolderInfo(parentFolderId);

            return [...displayedItem.properties].map((dip) =>
                dip.title === t("Location")
                    ? {
                          title: t("Location"),
                          content: (
                              <Link
                                  className="property-content"
                                  href={`/products/files/filter?folder=${parentFolderId}`}
                                  isHovered={true}
                              >
                                  {folderInfo.title}
                              </Link>
                          ),
                      }
                    : dip
            );
        };

        const updateLoadedItemAccess = async (selectedItem) => {
            const accesses = await getShareUsers(
                [selectedItem.folderId],
                [selectedItem.id]
            );

            const result = {
                owner: {},
                others: [],
            };

            accesses.forEach((access) => {
                const user = access.sharedTo;
                const userData = {
                    key: user.id,
                    img: user.avatarSmall,
                    link: user.profileUrl,
                    name: user.displayName,
                    email: user.email,
                };

                if (access.isOwner) result.owner = userData;
                else if (userData.email !== undefined)
                    result.others.push(userData);
            });

            return result;
        };

        const properties = await updateLoadedItemProperties(
            displayedItem,
            selectedItem
        );

        const access = await updateLoadedItemAccess(selectedItem);

        setItem({
            ...displayedItem,
            properties: properties,
            access: access,
        });
    };

    const showAllAccessUsers = () => {
        setIsShowAllAccessUsers(true);
    };

    const openSharingPanel = () => {
        const { id, isFolder } = item;

        onSelectItem({ id, isFolder });
        setSharingPanelVisible(true);
    };

    useEffect(() => {
        if (selectedItem.id !== item.id) updateItemsInfo(selectedItem);
    }, [selectedItem]);

    return (
        <>
            <StyledTitle>
                <ReactSVG className="icon" src={item.iconUrl} />
                <Text className="text" fontWeight={600} fontSize="16px">
                    {item.title}
                </Text>
            </StyledTitle>

            {selectedItem.thumbnailUrl ? (
                <StyledThumbnail>
                    <img src={item.thumbnailUrl} alt="" />
                </StyledThumbnail>
            ) : (
                <div className="no-thumbnail-img-wrapper">
                    <ReactSVG
                        className="no-thumbnail-img"
                        src={item.thumbnailUrl}
                    />
                </div>
            )}

            <StyledSubtitle>
                <Text fontWeight="600" fontSize="14px" color="#000000">
                    {t("SystemProperties")}
                </Text>
            </StyledSubtitle>

            <StyledProperties>
                {item.properties.map((p) => (
                    <div key={p.title} className="property">
                        <Text className="property-title">{p.title}</Text>
                        {p.content}
                    </div>
                ))}
            </StyledProperties>

            {!isRecycleBinFolder && (
                <>
                    <StyledSubtitle>
                        <Text fontWeight="600" fontSize="14px" color="#000000">
                            {t("WhoHasAccess")}
                        </Text>
                    </StyledSubtitle>

                    <StyledAccess>
                        <Tooltip
                            id="access-user-tooltip"
                            getContent={(dataTip) =>
                                dataTip ? (
                                    <Text fontSize="13px">{dataTip}</Text>
                                ) : null
                            }
                        />

                        <StyledAccessUser>
                            <div
                                data-for="access-user-tooltip"
                                data-tip={item.access.owner.name}
                            >
                                <a href={item.access.owner.link}>
                                    <img src={item.access.owner.img} />
                                </a>
                            </div>
                        </StyledAccessUser>

                        {item.access.others.length ? (
                            <div className="divider"></div>
                        ) : null}

                        {!isShowAllAccessUsers &&
                        item.access.others.length > 3 ? (
                            <>
                                {item.access.others.map((user, i) => {
                                    if (i < 3)
                                        return (
                                            <div key={user.key}>
                                                <StyledAccessUser>
                                                    <div
                                                        data-for="access-user-tooltip"
                                                        data-tip={user.name}
                                                    >
                                                        <a href={user.link}>
                                                            <img
                                                                src={user.img}
                                                            />
                                                        </a>
                                                    </div>
                                                </StyledAccessUser>
                                            </div>
                                        );
                                })}
                                <div
                                    className="show-more-users"
                                    onClick={showAllAccessUsers}
                                >
                                    {`+ ${item.access.others.length - 3} ${t(
                                        "Members"
                                    )}`}
                                </div>
                            </>
                        ) : (
                            <>
                                {item.access.others.map((user) => (
                                    <div key={user.key}>
                                        <StyledAccessUser>
                                            <div
                                                data-for="access-user-tooltip"
                                                data-tip={user.name}
                                            >
                                                <a href={user.link}>
                                                    <img src={user.img} />
                                                </a>
                                            </div>
                                        </StyledAccessUser>
                                    </div>
                                ))}
                            </>
                        )}
                    </StyledAccess>
                    <StyledOpenSharingPanel onClick={openSharingPanel}>
                        {t("OpenSharingSettings")}
                    </StyledOpenSharingPanel>
                </>
            )}
        </>
    );
};

export default inject(({}) => {
    return {};
})(
    withTranslation(["InfoPanel", "Home", "Common", "Translations"])(
        observer(SingleItem)
    )
);
