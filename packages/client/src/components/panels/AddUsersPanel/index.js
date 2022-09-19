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
  defaultAccess,
  onClose,
  onParentPanelClose,
  shareDataItems,
  tempDataItems,
  setDataItems,
  t,
  visible,
  groupsCaption,
  accessOptions,
  isMultiSelect,
  theme,
}) => {
  const accessRight = defaultAccess
    ? defaultAccess
    : isEncrypted
    ? ShareAccessRights.FullAccess
    : ShareAccessRights.ReadOnly;

  const onBackClick = () => onClose();

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

  const onUsersSelect = (users, access) => {
    const items = [];

    for (let item of users) {
      const currentItem = shareDataItems.find((x) => x.sharedTo.id === item.id);

      if (!currentItem) {
        const newItem = {
          access: access.access,
          email: item.email,
          id: item.id,
          displayName: item.label,
          avatar: item.avatar,
        };
        items.push(newItem);
      }
    }

    setDataItems(items);
    onClose();
  };

  const onUserSelect = (owner) => {
    const ownerItem = shareDataItems.find((x) => x.isOwner);
    ownerItem.sharedTo = owner[0];

    if (owner[0].key) {
      owner[0].id = owner[0].key;
    }

    setDataItems(shareDataItems);
    onClose();
  };

  const selectedAccess = accessOptions.filter(
    (access) => access.access === accessRight
  )[0];

  return (
    <>
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
        withoutBodyScroll
      >
        <PeopleSelector
          isMultiSelect={isMultiSelect}
          onAccept={onUsersSelect}
          onBackClick={onBackClick}
          accessRights={accessOptions}
          selectedAccessRight={selectedAccess}
          onCancel={onClosePanels}
          withCancelButton={!isMultiSelect}
          withAccessRights={isMultiSelect}
          withSelectAll={isMultiSelect}
        />
      </Aside>
    </>
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
