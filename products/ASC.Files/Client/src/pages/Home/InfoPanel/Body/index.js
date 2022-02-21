import RectangleLoader from "@appserver/common/components/Loaders/RectangleLoader";
import { FileType } from "@appserver/common/constants";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import Tooltip from "@appserver/components/tooltip";
import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { ReactSVG } from "react-svg";
import styled from "styled-components";

const StyledInfoRoomBody = styled.div`
    padding: 0px 16px 16px;
    .no-item {
        text-align: center;
    }
`;

const StyledItemTitle = styled.div`
    display: flex;
    flex-direction: row;
    align-items: center;
    width: 100%;
    height: 80px;

    .icon {
        svg {
            height: 32px;
            width: 32px;
        }
    }

    .text {
        font-family: Open Sans;
        line-height: 22px;
        margin: 0px 8px;
    }
`;

const StyledItemThumbnail = styled.div`
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
    height: 200px;
    padding: 4px;
    border: solid 1px #eceef1;
    border-radius: 6px;
    margin-bottom: 24px;
    img {
        max-height: 200px;
        max-width: 100%;
        width: auto;
        height: auto;
    }
`;

const StyledItemSubtitle = styled.div`
    display: flex;
    flex-direction: row;
    align-items: center;
    width: 100%;
    margin-bottom: 24px;
`;

const StyledItemProperties = styled.div`
    display: flex;
    flex-direction: column;
    width: 100%;
    margin-bottom: 24px;
    gap: 8px;

    .property {
        width: 100%;
        display: grid;
        grid-template-columns: 110px 1fr;
        grid-column-gap: 24px;

        .property-title {
            font-size: 13px;
            color: #333333;
        }

        .property-content {
            font-weight: 600;
            font-size: 13px;
            color: #333333;
        }
    }
`;

const StyledItemAccess = styled.div`
    display: flex;
    flex-direction: row;
    gap: 8px;
    align-items: center;
    .divider {
        background: #eceef1;
        margin: 2px 4px;
        width: 1px;
        height: 28px;
    }
`;

const StyleditemAccessUser = styled.div`
    width: 32px;
    height: 32px;
    border-radius: 50%;

    a {
        img {
            border-radius: 50%;
            width: 100%;
            height: 100%;
        }
    }
`;

const InfoPanelBodyContent = ({
    selectedItem,
    getFolderInfo,
    getIcon,
    getFolderIcon,

    getShareUsers,
}) => {
    const [item, setItem] = useState({});

    const updateItemsInfo = async (selectedItem) => {
        const displayedItem = {
            id: selectedItem.id,
            title: selectedItem.title,
            iconUrl: getItemIcon(selectedItem),
            thumbnailUrl: selectedItem.thumbnailUrl,
            properties: getItemProperties(selectedItem),
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

            return [...displayedItem.properties].map((op) =>
                op.title === "Location"
                    ? {
                          title: "Location",
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
                    : op
            );
        };

        const updateLoadedItemAccess = async (item) => {
            const accesses = await getShareUsers([item.folderId], [item.id]);

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
                else result.others.push(userData);
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

    const getItemIcon = (item) => {
        const extension = item.fileExst;
        const iconUrl = extension
            ? getIcon(24, extension)
            : getFolderIcon(item.providerKey, 24);

        return iconUrl;
    };

    const getItemProperties = (item) => {
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

            let hours = date.getHours(),
                month = normalize(date.getMonth()),
                day = normalize(date.getDate()),
                a_p = hours > 12 ? "AM" : "PM";

            if (hours === 0) hours = 12;
            else if (hours > 12) hours = hours - 12;

            return `${day}.${month}.${date.getFullYear()} ${hours}:${date.getMinutes()} ${a_p}`;
        };

        const getItemType = (fileType) => {
            switch (fileType) {
                case FileType.Unknown:
                    return "Unknown";
                case FileType.Archive:
                    return "Archive";
                case FileType.Video:
                    return "Video";
                case FileType.Audio:
                    return "Audio";
                case FileType.Image:
                    return "Image";
                case FileType.Spreadsheet:
                    return "Spreadsheet";
                case FileType.Presentation:
                    return "Presentation";
                case FileType.Document:
                    return "Document";

                default:
                    return "Folder";
            }
        };

        const itemSize = item.isFolder
            ? `${item.foldersCount} Folders | ${item.filesCount} Files`
            : item.contentLength;

        const itemType = getItemType(item.fileType);

        let result = [
            {
                title: "Owner",
                content: styledLink(
                    item.createdBy.displayName,
                    item.createdBy.profileUrl
                ),
            },
            {
                title: "Location",
                content: <RectangleLoader width="150" height="19" />,
            },
            {
                title: "Type",
                content: styledText(itemType),
            },
            {
                title: "Size",
                content: styledText(itemSize),
            },
            {
                title: "Date modified",
                content: styledText(parseAndFormatDate(item.updated)),
            },
            {
                title: "Last modified by",
                content: styledLink(
                    item.updatedBy.displayName,
                    item.updatedBy.profileUrl
                ),
            },
            {
                title: "Date creation",
                content: styledText(parseAndFormatDate(item.created)),
            },
        ];

        if (itemType === "Folder") return result;

        result.splice(3, 0, {
            title: "File extension",
            content: styledText(item.fileExst.split(".")[1].toUpperCase()),
        });

        result.push(
            {
                title: "Versions",
                content: styledText(item.version),
            },
            {
                title: "Comments",
                content: styledText(item.comment),
            }
        );

        return result;
    };

    useEffect(() => {
        if (selectedItem !== null) updateItemsInfo(selectedItem);
    }, [selectedItem]);

    return (
        <StyledInfoRoomBody>
            {!Object.keys(item).length ? (
                <div className="no-item">
                    <h4>Select an item to display it's info</h4>
                </div>
            ) : (
                <>
                    <StyledItemTitle>
                        <ReactSVG className="icon" src={item.iconUrl} />
                        <Text className="text" fontWeight={600} fontSize="16px">
                            {item.title}
                        </Text>
                    </StyledItemTitle>

                    {item.thumbnailUrl && (
                        <StyledItemThumbnail>
                            <img src={item.thumbnailUrl} alt="" />
                        </StyledItemThumbnail>
                    )}

                    <StyledItemSubtitle>
                        <Text fontWeight="600" fontSize="14px" color="#000000">
                            System Properties
                        </Text>
                    </StyledItemSubtitle>

                    <StyledItemProperties>
                        {item.properties.map((p) => (
                            <div key={p.title} className="property">
                                <Text className="property-title">
                                    {p.title}
                                </Text>
                                {p.content}
                            </div>
                        ))}
                    </StyledItemProperties>

                    <StyledItemSubtitle>
                        <Text fontWeight="600" fontSize="14px" color="#000000">
                            Who has access
                        </Text>
                    </StyledItemSubtitle>

                    <StyledItemAccess>
                        <StyleditemAccessUser>
                            <div
                                data-for="access-user-tooltip"
                                data-tip={item.access.owner.name}
                            >
                                <a href={item.access.owner.link}>
                                    <img src={item.access.owner.img} />
                                </a>
                            </div>
                        </StyleditemAccessUser>

                        {item.access.others.length ? (
                            <div className="divider"></div>
                        ) : null}

                        {item.access.others.map((user) => (
                            <div key={user.key}>
                                <StyleditemAccessUser>
                                    <div
                                        data-for="access-user-tooltip"
                                        data-tip={user.name}
                                    >
                                        <a href={user.link}>
                                            <img src={user.img} />
                                        </a>
                                    </div>
                                </StyleditemAccessUser>
                            </div>
                        ))}
                    </StyledItemAccess>

                    <Tooltip
                        id="access-user-tooltip"
                        getContent={(dataTip) =>
                            dataTip ? (
                                <Text fontSize="13px">{dataTip}</Text>
                            ) : null
                        }
                    />
                </>
            )}
        </StyledInfoRoomBody>
    );
};

export default inject(({ filesStore, formatsStore }) => {
    const selectedItem = JSON.parse(JSON.stringify(filesStore.bufferSelection));
    const { getFolderInfo, getShareUsers } = filesStore;

    const { getIcon, getFolderIcon } = formatsStore.iconFormatsStore;

    return {
        selectedItem,
        getFolderInfo,
        getShareUsers,

        getIcon,
        getFolderIcon,
    };
})(
    withRouter(
        withTranslation(["Home", "Common", "Translations"])(
            observer(InfoPanelBodyContent)
        )
    )
);
