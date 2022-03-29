import RectangleLoader from "@appserver/common/components/Loaders/RectangleLoader";
import { FileType } from "@appserver/common/constants";
import { LANGUAGE } from "@appserver/common/constants";
const moment = require("moment");

import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import Tooltip from "@appserver/components/tooltip";
import React, { useEffect, useState } from "react";
import { ReactSVG } from "react-svg";

import {
  StyledAccess,
  StyledAccessUser,
  StyledOpenSharingPanel,
  StyledProperties,
  StyledSubtitle,
  StyledThumbnail,
  StyledTitle,
} from "./styles/styles.js";

const SingleItem = (props) => {
  let {
    t,
    selectedItem,
    onSelectItem,
    setSharingPanelVisible,
    getFolderInfo,
    getIcon,
    getFolderIcon,
    getShareUsers,
    dontShowSize,
    dontShowLocation,
    dontShowAccess,
  } = props;

  let updateSubscription = true;
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
  const [showAccess, setShowAccess] = useState(false);
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
        return moment(date)
          .locale(localStorage.getItem(LANGUAGE))
          .format("DD.MM.YY hh:mm A");
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
        ? `${t("Translations:Folders")}: ${item.foldersCount} | ${t(
            "Translations:Files"
          )}: ${item.filesCount}`
        : item.contentLength;

      const itemType = getItemType(item.fileType);

      let result = [
        {
          id: "Owner",
          title: t("Common:Owner"),
          content: styledLink(
            item.createdBy?.displayName,
            item.createdBy?.profileUrl
          ),
        },
        {
          id: "Location",
          title: t("InfoPanel:Location"),
          content: <RectangleLoader width="150" height="19" />,
        },
        {
          id: "Type",
          title: t("Common:Type"),
          content: styledText(itemType),
        },
        {
          id: "Size",
          title: t("Common:Size"),
          content: styledText(itemSize),
        },
        {
          id: "ByLastModifiedDate",
          title: t("Home:ByLastModifiedDate"),
          content: styledText(parseAndFormatDate(item.updated)),
        },
        {
          id: "LastModifiedBy",
          title: t("LastModifiedBy"),
          content: styledLink(
            item.updatedBy?.displayName,
            item.updatedBy?.profileUrl
          ),
        },
        {
          id: "ByCreationDate",
          title: t("Home:ByCreationDate"),
          content: styledText(parseAndFormatDate(item.created)),
        },
      ];

      if (item.isFolder) return result;

      result.splice(3, 0, {
        id: "FileExtension",
        title: t("FileExtension"),
        content: styledText(item.fileExst?.split(".")[1].toUpperCase()),
      });

      result.push(
        {
          id: "Versions",
          title: t("Versions"),
          content: styledText(item.version),
        },
        {
          id: "Comments",
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
      thumbnailUrl: selectedItem.thumbnailUrl || getItemIcon(selectedItem, 96),
      properties: getSingleItemProperties(selectedItem),
      access: {
        owner: {
          img: selectedItem.createdBy?.avatarSmall,
          link: selectedItem.createdBy?.profileUrl,
        },
        others: [],
      },
    };

    setItem(displayedItem);
    await loadAsyncData(displayedItem, selectedItem);
  };

  const loadAsyncData = async (displayedItem, selectedItem) => {
    if (!updateSubscription) return;

    const updateLoadedItemProperties = async (displayedItem, selectedItem) => {
      const parentFolderId = selectedItem.isFolder
        ? selectedItem.parentId
        : selectedItem.folderId;

      const noLocationProperties = [...displayedItem.properties].filter(
        (dip) => dip.id !== "Location"
      );

      let result;
      await getFolderInfo(parentFolderId)
        .catch(() => {
          result = noLocationProperties;
        })
        .then((data) => {
          if (!data) {
            result = noLocationProperties;
            return;
          }
          result = [...displayedItem.properties].map((dip) =>
            dip.id === "Location"
              ? {
                  id: "Location",
                  title: t("Location"),
                  content: (
                    <Link
                      className="property-content"
                      href={`/products/files/filter?folder=${parentFolderId}`}
                      isHovered={true}
                    >
                      {data.title}
                    </Link>
                  ),
                }
              : dip
          );
        });

      return result;
    };

    const updateLoadedItemAccess = async (selectedItem) => {
      const parentFolderId = selectedItem.isFolder
        ? selectedItem.parentId
        : selectedItem.folderId;

      let result;
      await getShareUsers([parentFolderId], [selectedItem.id])
        .catch((e) => {
          setShowAccess(false);
        })
        .then((accesses) => {
          if (!accesses) {
            setShowAccess(false);
            return;
          }
          const accessResult = {
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

            if (access.isOwner) accessResult.owner = userData;
            else if (userData.email !== undefined)
              accessResult.others.push(userData);
          });

          result = accessResult;
        });
      return result;
    };

    const properties = await updateLoadedItemProperties(
      displayedItem,
      selectedItem
    );

    let access;
    if (!dontShowAccess) {
      access = await updateLoadedItemAccess(selectedItem);
    }

    if (updateSubscription) {
      setItem({
        ...displayedItem,
        properties: properties,
        access: access,
      });
      if (access) setShowAccess(true);
    }
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
    if (selectedItem.id !== item.id && updateSubscription)
      updateItemsInfo(selectedItem);
    return () => (updateSubscription = false);
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
          <ReactSVG className="no-thumbnail-img" src={item.thumbnailUrl} />
        </div>
      )}

      <StyledSubtitle>
        <Text fontWeight="600" fontSize="14px">
          {t("SystemProperties")}
        </Text>
      </StyledSubtitle>

      <StyledProperties>
        {item.properties.map((p) => {
          if (dontShowSize && p.id === "Size") return;
          if (dontShowLocation && p.id === "Location") return;
          return (
            <div key={p.title} className="property">
              <Text className="property-title">{p.title}</Text>
              {p.content}
            </div>
          );
        })}
      </StyledProperties>

      {showAccess && (
        <>
          <StyledSubtitle>
            <Text fontWeight="600" fontSize="14px">
              {t("WhoHasAccess")}
            </Text>
          </StyledSubtitle>

          <StyledAccess>
            <Tooltip
              id="access-user-tooltip"
              getContent={(dataTip) =>
                dataTip ? <Text fontSize="13px">{dataTip}</Text> : null
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

            {item.access.others.length ? <div className="divider"></div> : null}

            {!isShowAllAccessUsers && item.access.others.length > 3 ? (
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
                              <img src={user.img} />
                            </a>
                          </div>
                        </StyledAccessUser>
                      </div>
                    );
                })}
                <div className="show-more-users" onClick={showAllAccessUsers}>
                  {`+ ${item.access.others.length - 3} ${t("Members")}`}
                </div>
              </>
            ) : (
              <>
                {item.access.others.map((user) => (
                  <div key={user.key}>
                    <StyledAccessUser>
                      <div data-for="access-user-tooltip" data-tip={user.name}>
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

export default SingleItem;
