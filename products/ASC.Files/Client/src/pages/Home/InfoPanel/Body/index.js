import { FileType } from "@appserver/common/constants";
import IconButton from "@appserver/components/icon-button";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import styled from "styled-components";

const StyledInfoRoomBody = styled.div`
    .item {
        padding: 8px 0;
    }
`;

const StyledItemTitle = styled.div`
    display: flex;
    flex-direction: row;
    align-items: center;
    width: 100%;
    height: 80px;

    .icon {
        width: 32px;
        height: 32px;
        margin: 0px 8px;
        display: inline-block;
    }

    .text {
        font-family: Open Sans;
        line-height: 22px;
        margin: 0px 8px;
    }
`;

const StyledItemSubtitle = styled.div`
    display: flex;
    flex-direction: row;
    align-items: center;
    width: 100%;
    padding: 24px 0;
`;

const StyledItemProperties = styled.div`
    .property {
        width: 100%;
        display: grid;
        grid-template-columns: 110px 1fr;
        .property-title {
            font-size: 13px;
            color: #333333;
            margin: 8px 0px;
        }

        .property-content {
            font-weight: 600;
            font-size: 13px;
            color: #333333;
            margin: 8px 0px;
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

const InfoPanelBodyContent = ({ selection, getFolderInfo }) => {
    const selectedItems = selection;
    console.log(selectedItems);
    const [items, setItems] = useState([
        {
            title: "",
            icon: "",
            properties: [],
            access: {
                owner: {
                    img: "",
                    link: "",
                },
                others: [],
            },
        },
    ]);

    const updateItemsInfo = async () => {
        const res = await Promise.all(
            selectedItems.map(async (s) => {
                const existingItem = items.find((i) => i.id === s.id);
                if (existingItem) return existingItem;

                const properties = await getItemProperties(s);
                const access = getItemAccess(s);

                console.log(access);
                return {
                    id: s.id,
                    title: s.title,
                    icon: s.icon,
                    properties: properties,
                    access: access,
                };
            })
        );

        setItems(res);
    };

    const getItemProperties = async (item) => {
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

        console.log(item);

        const parentFolderId = item.isFolder ? item.parentId : item.folderId;
        const folderInfo = await getFolderInfo(parentFolderId);
        console.log(folderInfo);

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
                content: styledLink(
                    folderInfo.title,
                    `/products/files/filter?folder=${parentFolderId}`
                ),
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

    const getItemAccess = (item) => {
        let result = {
            owner: {
                img: item.createdBy.avatarSmall,
                link: item.createdBy.profileUrl,
            },
            others: [],
        };

        return result;

        if (item.access === 0) return result;
    };

    useEffect(() => {
        updateItemsInfo();
    }, [selectedItems]);

    return (
        <StyledInfoRoomBody>
            {items.map((i) => (
                <div className="item" key={i.id}>
                    <StyledItemTitle>
                        <IconButton
                            className="icon"
                            iconName={i.icon}
                            size="32"
                        />
                        <Text
                            className="text"
                            fontWeight={600}
                            font-size="16px"
                        >
                            {i.title}
                        </Text>
                    </StyledItemTitle>

                    {/* < THUMBNAIL /> */}

                    <StyledItemSubtitle>
                        <Text fontWeight="600" fontSize="14px" color="#000000">
                            System Properties
                        </Text>
                    </StyledItemSubtitle>

                    <StyledItemProperties>
                        {i.properties.map((p) => (
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
                            <a href={i.access.owner.link}>
                                <img src={i.access.owner.img} />
                            </a>
                        </StyleditemAccessUser>
                        {!i.access.others && <div className="divider"></div>}
                    </StyledItemAccess>
                </div>
            ))}
        </StyledInfoRoomBody>
    );
};

export default inject(({ filesStore }) => {
    const selection = JSON.parse(JSON.stringify(filesStore.selection));
    const { getFolderInfo } = filesStore;

    return { selection, getFolderInfo };
})(
    withRouter(
        withTranslation(["Home", "Common", "Translations"])(
            observer(InfoPanelBodyContent)
        )
    )
);
