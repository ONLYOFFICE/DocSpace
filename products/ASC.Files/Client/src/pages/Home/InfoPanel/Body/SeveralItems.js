import RectangleLoader from "@appserver/common/components/Loaders/RectangleLoader";
import { FileType } from "@appserver/common/constants";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import {
    StyledProperties,
    StyledSubtitle,
    StyledTitle,
} from "./styles/styles.js";

const SeveralItems = (props) => {
    const { t, selectedItems, getIcon, getFolderInfo } = props;
    const itemsIcon = getIcon(24, ".file");

    let items = [];
    const [mixedProperties, setMixedProperties] = useState([]);

    const systemPropertiesTemplate = [
        {
            title: t("Common:Owner"),
            content: "",
        },
        {
            title: t("InfoPanel:Location"),
            content: "",
        },
        {
            title: t("Common:Type"),
            content: "",
        },
        {
            title: t("Common:Size"),
            content: "",
        },
        {
            title: t("Home:ByLastModifiedDate"),
            content: "",
        },
        {
            title: t("LastModifiedBy"),
            content: "",
        },
        {
            title: t("Home:ByCreationDate"),
            content: "",
        },
    ];

    const updateItems = (selectedItems) => {
        let newProperties = selectedItems.map((selectedItem) =>
            getSingleItemProperties(selectedItem)
        );

        let mixedProperties = mixProperties(newProperties);
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

    const mixProperties = (properties) => {
        const result = [...systemPropertiesTemplate];
        result.forEach((finalProperty, i) => {
            const itemsProperties = properties.map((itemProperties) => {
                const [property] = itemProperties.filter((obj) => {
                    return obj.title === finalProperty.title;
                });
                console.log(property, finalProperty);
                return itemProperties[i].title === finalProperty.title
                    ? itemProperties[i].content
                    : null;
            });

            //console.log(itemsProperties);
        });
    };

    useEffect(() => {
        if (selectedItems.length !== items.length) updateItems(selectedItems);
    }, [selectedItems]);

    return (
        <>
            <StyledTitle>
                <ReactSVG className="icon" src={itemsIcon} />
                <Text className="text" fontWeight={600} fontSize="16px">
                    {`${t("ItemsSelected")}: ${selectedItems.length}`}
                </Text>
            </StyledTitle>

            <div className="no-thumbnail-img-wrapper">
                <img
                    className="no-thumbnail-img"
                    src="images/empty_screen.png"
                />
            </div>

            <StyledSubtitle>
                <Text fontWeight="600" fontSize="14px" color="#000000">
                    {t("SystemProperties")}
                </Text>
            </StyledSubtitle>

            <StyledProperties>
                {mixedProperties.map((p) => (
                    <div key={p.title} className="property">
                        <Text className="property-title">{p.title}</Text>
                        {p.content}
                    </div>
                ))}
            </StyledProperties>
        </>
    );
};

export default inject(({}) => {
    return {};
})(withTranslation(["InfoPanel"])(observer(SeveralItems)));
