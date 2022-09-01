import React, { useEffect, useState } from "react";

import { FileType } from "@docspace/common/constants";

import Text from "@docspace/components/text";
import Tooltip from "@docspace/components/tooltip";

import InfoHelper from "./helpers/InfoHelper.js";
import {
  StyledAccess,
  StyledAccessItem,
  StyledOpenSharingPanel,
  StyledThumbnail,
} from "../../styles/info.js";
import { StyledProperties, StyledSubtitle } from "../../styles/common.js";

const SingleItem = ({
  t,
  selectedItem,
  onSelectItem,
  setSharingPanelVisible,
  getIcon,
  getFolderIcon,
  getShareUsers,
  dontShowSize,
  dontShowLocation,
  dontShowAccess,
  personal,
  createThumbnail,
  culture,
}) => {
  let infoHelper;

  const [itemProperties, setItemProperties] = useState([]);
  const [itemAccess, setItemAccess] = useState(null);

  // {
  //   owner: {
  //     img: "",
  //     link: "",
  //   },
  //   others: [],
  // }

  const updateItemsInfo = async (selectedItem) => {
    setItemProperties(infoHelper.getPropertyList());

    // setItemAccess({
    //   access: {
    //     owner: {
    //       img: selectedItem.createdBy?.avatarSmall,
    //       link: selectedItem.createdBy?.profileUrl,
    //     },
    //     others: [],
    //   },
    // });

    await loadAsyncData();
  };

  const loadAsyncData = async () => {
    if (
      !selectedItem.thumbnailUrl &&
      !selectedItem.isFolder &&
      selectedItem.thumbnailStatus === 0 &&
      (selectedItem.fileType === FileType.Image ||
        selectedItem.fileType === FileType.Spreadsheet ||
        selectedItem.fileType === FileType.Presentation ||
        selectedItem.fileType === FileType.Document)
    ) {
      await createThumbnail(selectedItem.id);
    }

    // const updateLoadedItemProperties = async (displayedItem, selectedItem) => {
    //   const parentFolderId = selectedItem.isFolder
    //     ? selectedItem.parentId
    //     : selectedItem.folderId;

    //   const noLocationProperties = [...displayedItem.properties].filter(
    //     (dip) => dip.id !== "Location"
    //   );

    //   let result;
    //   await getFolderInfo(parentFolderId)
    //     .catch(() => {
    //       result = noLocationProperties;
    //     })
    //     .then((data) => {
    //       if (!data) {
    //         result = noLocationProperties;
    //         return;
    //       }
    //       result = [...displayedItem.properties].map((dip) =>
    //         dip.id === "Location"
    //           ? {
    //               id: "Location",
    //               title: t("Common:Location"),
    //               content: (
    //                 <Link
    //                   className="property-content"
    //                   href={`/products/files/filter?folder=${parentFolderId}`}
    //                   isHovered={true}
    //                 >
    //                   {data.title}
    //                 </Link>
    //               ),
    //             }
    //           : dip
    //       );
    //     });

    //   return result;
    // };

    //   const updateLoadedItemAccess = async (selectedItem) => {
    //     const accesses = await getShareUsers(
    //       [selectedItem.isFolder ? selectedItem.parentId : selectedItem.folderId],
    //       [selectedItem.id]
    //     );

    //     const result = {
    //       owner: {},
    //       others: [],
    //     };

    //     accesses.forEach((access) => {
    //       let key = access.sharedTo.id,
    //         img = access.sharedTo.avatarSmall,
    //         link = access.sharedTo.profileUrl,
    //         name = access.sharedTo.displayName || access.sharedTo.name,
    //         { manager } = access.sharedTo;

    //       if (access.isOwner) result.owner = { key, img, link, name };
    //       else {
    //         if (access.sharedTo.email)
    //           result.others.push({ key, type: "user", img, link, name });
    //         else if (acc ess.sharedTo.manager)
    //           result.others.push({ key, type: "group", name, manager });
    //       }
    //     });

    //     result.others = result.others.sort((a) => (a.type === "group" ? -1 : 1));
    //     return result;
    //   };

    //   const properties = await updateLoadedItemProperties(
    //     displayedItem,
    //     selectedItem
    //   );

    //   if (dontShowAccess) {
    //     setItem({
    //       // ...displayedItem,
    //       //properties: properties,
    //     });
    //     return;
    //   }

    //   if (!personal) {
    //     const access = await updateLoadedItemAccess(selectedItem);
    //     setItem({
    //       // ...displayedItem,
    //       // properties: properties,
    //       access: access,
    //     });
    //   }
  };

  const openSharingPanel = () => {
    const { id, isFolder } = selectedItem;
    onSelectItem({ id, isFolder });
    setSharingPanelVisible(true);
  };

  useEffect(() => {
    infoHelper = new InfoHelper(
      t,
      selectedItem,
      personal,
      culture,
      getFolderIcon,
      getIcon
    );
    updateItemsInfo(selectedItem);
  }, [selectedItem]);

  return (
    <>
      {selectedItem?.hasCustonThumbnail ? (
        <StyledThumbnail>
          <img
            src={selectedItem.thumbnailUrl}
            alt="thumbnail-image"
            height={260}
            width={360}
          />
        </StyledThumbnail>
      ) : (
        <div className="no-thumbnail-img-wrapper">
          <img
            className={`no-thumbnail-img ${selectedItem.isRoom && "is-room"}`}
            src={selectedItem.thumbnailUrl}
            alt="thumbnail-icon-big"
          />
        </div>
      )}

      <StyledSubtitle>
        <Text fontWeight="600" fontSize="14px">
          {t("SystemProperties")}
        </Text>
      </StyledSubtitle>

      <StyledProperties>
        {itemProperties.map((p) => {
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

      {!!itemAccess && !dontShowAccess && !personal && (
        <>
          <StyledSubtitle>
            <Text fontWeight="600" fontSize="14px">
              {t("WhoHasAccess")}
            </Text>
          </StyledSubtitle>

          <StyledAccess>
            <Tooltip
              id="access-item-tooltip"
              getContent={(dataTip) =>
                dataTip ? <Text fontSize="13px">{dataTip}</Text> : null
              }
            />

            <StyledAccessItem>
              <div
                data-for="access-item-tooltip"
                className="access-item-tooltip"
                data-tip={itemAccess.owner.name}
              >
                <div className="item-user">
                  <a href={itemAccess.owner.link}>
                    <img src={itemAccess.owner.img} />
                  </a>
                </div>
              </div>
            </StyledAccessItem>

            {itemAccess.others.length > 0 && <div className="divider"></div>}

            {itemAccess.others.map((item, i) => {
              if (i < 3)
                return (
                  <div key={item.key}>
                    <StyledAccessItem>
                      <div
                        data-for="access-item-tooltip"
                        data-tip={item.name}
                        className="access-item-tooltip"
                      >
                        {item.type === "user" ? (
                          <div className="item-user">
                            <a href={item.link}>
                              <img src={item.img} />
                            </a>
                          </div>
                        ) : (
                          <div className="item-group">
                            <span>{item.name.substr(0, 2).toUpperCase()}</span>
                          </div>
                        )}
                      </div>
                    </StyledAccessItem>
                  </div>
                );
            })}

            {itemAccess.others.length > 3 && (
              <div className="show-more-users" onClick={openSharingPanel}>
                {`+ ${itemAccess.others.length - 3} ${t("Members")}`}
              </div>
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
