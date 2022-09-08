import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import Backdrop from "@docspace/components/backdrop";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import IconButton from "@docspace/components/icon-button";
import { ShareAccessRights } from "@docspace/common/constants";
import PeopleSelector from "@docspace/client/src/components/PeopleSelector";
import { withTranslation } from "react-i18next";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";

const AddUsersPanel = ({
  isEncrypted,
  onClose,
  onParentPanelClose,
  shareDataItems,
  setShareDataItems,
  t,
  visible,
  groupsCaption,
  accessOptions,
  isMultiSelect,
  theme,
}) => {
  const accessRight = isEncrypted
    ? ShareAccessRights.FullAccess
    : ShareAccessRights.ReadOnly;

  const onArrowClick = () => onClose();

  const onKeyPress = (e) => {
    if (e.key === "Esc" || e.key === "Escape") onClose();
  };

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);

    return () => window.removeEventListener("keyup", onKeyPress);
  });

  const onClosePanels = () => {
    onClose();
    onParentPanelClose();
  };

  const onPeopleSelect = (users) => {
    const items = shareDataItems;
    for (let item of users) {
      const currentItem = shareDataItems.find((x) => x.sharedTo.id === item.id);

      if (!currentItem) {
        const newItem = {
          access: accessRight,
          isLocked: false,
          isOwner: false,
          sharedTo: item,
        };
        items.push(newItem);
      }
    }

    setShareDataItems(items);
    onClose();
  };

  const onOwnerSelect = (owner) => {
    const ownerItem = shareDataItems.find((x) => x.isOwner);
    ownerItem.sharedTo = owner[0];

    if (owner[0].key) {
      owner[0].id = owner[0].key;
    }

    setShareDataItems(shareDataItems);
    onClose();
  };

  const accessRights = accessOptions.map((access) => {
    return {
      key: access,
      label: t(access),
    };
  });

  const selectedAccess = accesses.filter(
    (access) => access.key === "Review"
  )[0];

  return (
    <div visible={visible}>
      <Backdrop
        onClick={onClosePanels}
        visible={visible}
        zIndex={310}
        isAside={true}
      />
      <Aside
        className="header_aside-panel"
        visible={visible}
        onClose={onClosePanels}
      >
        <PeopleSelector
          isMultiSelect={isMultiSelect}
          onAccept={isMultiSelect ? onPeopleSelect : onOwnerSelect}
          onBackClick={onArrowClick}
          headerLabel={
            isMultiSelect
              ? t("Common:AddUsers")
              : t("PeopleTranslations:OwnerChange")
          }
          accessRights={accessRights}
          selectedAccessRight={selectedAccess}
          onCancel={onClosePanels}
          withCancelButton={!isMultiSelect}
          withAccessRights={isMultiSelect}
          withSelectAll={isMultiSelect}
        />
      </Aside>
    </div>
  );
};

AddUsersPanel.propTypes = {
  visible: PropTypes.bool,
  onParentPanelClose: PropTypes.func,
  onClose: PropTypes.func,
};

export default inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(
  observer(
    withTranslation(["SharingPanel", "PeopleTranslations", "Common"])(
      withLoader(AddUsersPanel)(<Loaders.DialogAsideLoader isPanel />)
    )
  )
);
