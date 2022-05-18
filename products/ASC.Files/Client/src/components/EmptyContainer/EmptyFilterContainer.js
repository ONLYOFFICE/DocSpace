import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import EmptyContainer from "./EmptyContainer";
import FilesFilter from "@appserver/common/api/files/filter";
import Link from "@appserver/components/link";
import IconButton from "@appserver/components/icon-button";
import toastr from "studio/toastr";

const EmptyFilterContainer = ({
  t,
  selectedFolderId,
  setIsLoading,
  fetchFiles,
  linkStyles,
}) => {
  const subheadingText = t("EmptyFilterSubheadingText");
  const descriptionText = t("EmptyFilterDescriptionText");

  const onResetFilter = () => {
    setIsLoading(true);
    const newFilter = FilesFilter.getDefault();
    fetchFiles(selectedFolderId, newFilter)
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  const buttons = (
    <div className="empty-folder_container-links">
      <IconButton
        className="empty-folder_container-icon"
        size="12"
        onClick={onResetFilter}
        iconName="/static/images/clear.empty.filter.svg"
        isFill
      />
      <Link onClick={onResetFilter} {...linkStyles}>
        {t("Common:ClearFilter")}
      </Link>
    </div>
  );

  return (
    <EmptyContainer
      headerText={t("Common:NotFoundTitle")}
      descriptionText={descriptionText}
      imageSrc="images/empty_screen_filter_alt.svg"
      buttons={buttons}
    />
  );
};

export default inject(({ filesStore, selectedFolderStore }) => ({
  fetchFiles: filesStore.fetchFiles,
  selectedFolderId: selectedFolderStore.id,
  setIsLoading: filesStore.setIsLoading,
}))(withTranslation(["Home", "Common"])(observer(EmptyFilterContainer)));
