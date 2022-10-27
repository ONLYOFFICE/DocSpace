import React, { useRef } from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import { Text } from "@docspace/components";
import { ItemIcon } from "@docspace/components";

import ItemContextOptions from "./ItemContextOptions";

import { StyledTitle } from "../../styles/common";

const FilesItemTitle = ({
  t,

  selection,
  isSeveralItems,

  setBufferSelection,
  getIcon,

  getContextOptions,
  getContextOptionActions,
  severalItemsLength,

  selectedFolderId,
}) => {
  if (isSeveralItems)
    return (
      <StyledTitle>
        <div className="item-icon">
          <ReactSVG className="icon" src={getIcon(32, ".file")} />
        </div>
        <Text className="text">
          {`${t("InfoPanel:ItemsSelected")}: ${severalItemsLength}`}
        </Text>
      </StyledTitle>
    );

  const itemTitleRef = useRef();

  return (
    <StyledTitle ref={itemTitleRef}>
      <div className="item-icon">
        <ItemIcon item={selection} />
      </div>
      <Text className="text">{selection.title}</Text>
      {selection && (
        <ItemContextOptions
          t={t}
          itemTitleRef={itemTitleRef}
          selection={selection}
          setBufferSelection={setBufferSelection}
          getContextOptions={getContextOptions}
          getContextOptionActions={getContextOptionActions}
          selectedFolderId={selectedFolderId}
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
])(FilesItemTitle);
