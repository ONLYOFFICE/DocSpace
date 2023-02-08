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
}) => {
  const onChangeEmailSubscription = async (e) => {
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
        <Text
          fontSize="15px"
          fontWeight="600"
          className="subscription-title"
          noSelect
        >
          {t("RoomsActions")}
        </Text>
        <Text fontSize="12px">{t("ActionsWithFilesDescription")}</Text>
      </div>
      <ToggleButton
        className="toggle-btn"
        onChange={onChangeEmailSubscription}
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
