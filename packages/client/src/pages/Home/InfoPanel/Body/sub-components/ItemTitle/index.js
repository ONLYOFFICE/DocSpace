import React from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import { Text } from "@docspace/components";

import ItemContextOptions from "./ItemContextOptions";

import { StyledTitle } from "../../styles/common";
import AccountsItemTitle from "./AccountsItemTitle";
import FilesItemTitle from "./FilesItemTitle";
import GalleryItemTitle from "./GalleryItemTitle";

const ItemTitle = ({
  t,

  selection,
  selectionParentRoom,
  roomsView,

  isRooms,
  isAccounts,
  isGallery,
  isSeveralItems,

  setBufferSelection,
  getIcon,

  getContextOptions,
  getContextOptionActions,
  getUserContextOptions,
}) => {
  if (isAccounts)
    return (
      <AccountsItemTitle
        selection={selection}
        isSeveralItems={isSeveralItems}
        getUserContextOptions={getUserContextOptions}
      />
    );

  if (isGallery)
    return <GalleryItemTitle selection={selection} getIcon={getIcon} />;

  const filesItemSelection =
    isRooms &&
    !isSeveralItems &&
    roomsView === "members" &&
    !selection.isRoom &&
    !!selectionParentRoom
      ? selectionParentRoom
      : selection;

  return (
    <FilesItemTitle
      selection={filesItemSelection}
      isSeveralItems={isSeveralItems}
      setBufferSelection={setBufferSelection}
      getIcon={getIcon}
      getContextOptions={getContextOptions}
      getContextOptionActions={getContextOptionActions}
    />
  );

  if (isSeveralItems)
    return (
      <StyledTitle>
        <ReactSVG className="icon" src={getIcon(32, ".file")} />
        <Text className="text">
          {`${t("InfoPanel:ItemsSelected")}: ${displayedSelection.length}`}
        </Text>
      </StyledTitle>
    );
  return isGallery ? (
    <StyledTitle>
      <ReactSVG className="icon" src={getIcon(32, ".docxf")} />
      <Text className="text">{displayedSelection?.attributes?.name_form}</Text>
    </StyledTitle>
  ) : (
    <StyledTitle>
      <img
        className={`icon ${displayedSelection.isRoom && "is-room"}`}
        src={displayedSelection.icon}
        alt="thumbnail-icon"
      />
      <Text className="text">{displayedSelection.title}</Text>
      {displayedSelection && (
        <ItemContextOptions
          t={t}
          selection={displayedSelection}
          setBufferSelection={setBufferSelection}
          getContextOptions={getContextOptions}
          getContextOptionActions={getContextOptionActions}
        />
      )}
    </StyledTitle>
  );
};

export default withTranslation([
  "Files",
  "Common",
  "Translations",
  "InfoPanel",
  "SharingPanel",
])(ItemTitle);

// const ItemTitle = ({
//   t,

//   selection,
//   selectionParentRoom,

//   isGallery,
//   isSeveralItems,
//   setBufferSelection,
//   getIcon,
//   getContextOptions,
//   getContextOptionActions,
// }) => {
//   return isSeveralItems ? (
//     <StyledTitle>
//       <ReactSVG className="icon" src={getIcon(32, ".file")} />
//       <Text className="text">
//         {`${t("InfoPanel:ItemsSelected")}: ${selection.length}`}
//       </Text>
//     </StyledTitle>
//   ) : isGallery ? (
//     <StyledTitle>
//       <ReactSVG className="icon" src={getIcon(32, ".docxf")} />
//       <Text className="text">{selection?.attributes?.name_form}</Text>
//     </StyledTitle>
//   ) : (
//     <StyledTitle>
//       <img
//         className={`icon ${selection.isRoom && "is-room"}`}
//         src={selection.icon}
//         alt="thumbnail-icon"
//       />
//       <Text className="text">{selection.title}</Text>
//       {selection && (
//         <ItemContextOptions
//           t={t}
//           selection={selection}
//           setBufferSelection={setBufferSelection}
//           getContextOptions={getContextOptions}
//           getContextOptionActions={getContextOptionActions}
//         />
//       )}
//     </StyledTitle>
//   );
// };

// export default withTranslation([
//   "Files",
//   "Common",
//   "Translations",
//   "InfoPanel",
//   "SharingPanel",
// ])(ItemTitle);
