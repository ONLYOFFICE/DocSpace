import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";

const EmptyFolderContainer = ({
  t,
  onCreate,
  fetchFiles,
  fetchRooms,
  setIsLoading,
  parentId,
  linkStyles,
  isRooms,
}) => {
  const onBackToParentFolder = () => {
    setIsLoading(true);

    isRooms
      ? fetchRooms(parentId).finally(() => setIsLoading(false))
      : fetchFiles(parentId).finally(() => setIsLoading(false));
  };

  const buttons = (
    <>
      <div className="empty-folder_container-links">
        <img
          className="empty-folder_container_plus-image"
          src="images/plus.svg"
          data-format="docx"
          onClick={onCreate}
          alt="plus_icon"
        />
        <Box className="flex-wrapper_container">
          <Link data-format="docx" onClick={onCreate} {...linkStyles}>
            {t("Document")},
          </Link>
          <Link data-format="xlsx" onClick={onCreate} {...linkStyles}>
            {t("Spreadsheet")},
          </Link>
          <Link data-format="pptx" onClick={onCreate} {...linkStyles}>
            {t("Presentation")},
          </Link>
          <Link data-format="docxf" onClick={onCreate} {...linkStyles}>
            {t("Translations:NewForm")}
          </Link>
        </Box>
      </div>

      <div className="empty-folder_container-links">
        <img
          className="empty-folder_container_plus-image"
          src="images/plus.svg"
          onClick={onCreate}
          alt="plus_icon"
        />
        <Link {...linkStyles} onClick={onCreate}>
          {t("Folder")}
        </Link>
      </div>

      <div className="empty-folder_container-links">
        <img
          className="empty-folder_container_up-image"
          src="images/up.svg"
          onClick={onBackToParentFolder}
          alt="up_icon"
        />

        <Link onClick={onBackToParentFolder} {...linkStyles}>
          {t("BackToParentFolderButton")}
        </Link>
      </div>
    </>
  );

  return (
    <EmptyContainer
      headerText={t("EmptyScreenFolder")}
      style={{ gridColumnGap: "39px" }}
      descriptionText={t("EmptyFolderDecription")}
      imageSrc="/static/images/empty_screen_alt.svg"
      buttons={buttons}
    />
  );
};

export default inject(({ filesStore, selectedFolderStore }) => {
  const { fetchFiles, fetchRooms } = filesStore;
  const { navigationPath, parentId } = selectedFolderStore;

  let isRootRoom, isRoom, id;
  if (navigationPath.length) {
    isRootRoom = navigationPath.at(-1).isRootRoom;
    isRoom = navigationPath.at(-1).isRoom;
    id = navigationPath.at(-1).id;
  }

  return {
    fetchFiles,
    fetchRooms,
    setIsLoading: filesStore.setIsLoading,
    parentId: id ?? parentId,
    isRooms: isRoom || isRootRoom,
  };
})(withTranslation(["Files", "Translations"])(observer(EmptyFolderContainer)));
