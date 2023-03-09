import { inject, observer } from "mobx-react";
import { runInAction } from "mobx";
import React from "react";

import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
import { NotificationsType } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";
const RoomsActionsContainer = ({
  t,
  badgesSubscription,
  changeSubscription,
  fetchTreeFolders,
  treeFolders,
  textProps,
  textDescriptionsProps,
}) => {
  const onChangeBadgeSubscription = async (e) => {
    const checked = e.currentTarget.checked;
    try {
      await changeSubscription(NotificationsType.Badges, checked);

      runInAction(() => {
        !checked &&
          treeFolders.map((item) => {
            return (item.newItems = 0);
          });
      });

      await fetchTreeFolders();
    } catch (e) {
      toastr.error(e);
    }
  };

  return (
    <div className="notification-container">
      <div>
        <Text {...textProps} className="subscription-title">
          {t("RoomsActions")}
        </Text>
        <Text {...textDescriptionsProps}>
          {t("ActionsWithFilesDescription")}
        </Text>
      </div>
      <ToggleButton
        className="toggle-btn"
        onChange={onChangeBadgeSubscription}
        isChecked={badgesSubscription}
      />
    </div>
  );
};

export default inject(({ peopleStore, treeFoldersStore }) => {
  const { targetUserStore } = peopleStore;
  const { fetchTreeFolders, treeFolders } = treeFoldersStore;
  const { changeSubscription, badgesSubscription } = targetUserStore;

  return {
    treeFolders,
    fetchTreeFolders,
    changeSubscription,
    badgesSubscription,
  };
})(observer(RoomsActionsContainer));
