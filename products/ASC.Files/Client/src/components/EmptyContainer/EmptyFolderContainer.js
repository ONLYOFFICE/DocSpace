import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@appserver/components/link";
import Box from "@appserver/components/box";

const EmptyFolderContainer = ({
  t,
  onCreate,
  filter,
  fetchFiles,
  setIsLoading,
  parentId,
  linkStyles,
}) => {
  const onBackToParentFolder = () => {
    const newFilter = filter.clone();
    setIsLoading(true);
    fetchFiles(parentId, newFilter).finally(() => setIsLoading(false));
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
            {t("Presentation")}
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
      headerText={t("EmptyFolderHeader")}
      imageSrc="/static/images/empty_screen.png"
      buttons={buttons}
    />
  );
};

export default inject(({ filesStore, selectedFolderStore }) => {
  const { filter, fetchFiles } = filesStore;

  return {
    filter,
    fetchFiles,
    setIsLoading: filesStore.setIsLoading,
    parentId: selectedFolderStore.parentId,
  };
})(withTranslation("Home")(observer(EmptyFolderContainer)));
