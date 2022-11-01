import { inject, observer } from "mobx-react";
import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { toastr } from "@docspace/components";
const ButtonsContainer = (props) => {
  const {
    changeEmailSubscription,
    t,
    onClosePanel,
    dailyFeedEmail,
    isDailyFeedEmailSubscription,
    tipsSubscription,
    tipsEmail,
    roomsActionsBadge,
  } = props;

  const onSavingChanges = async () => {
    const requests = [];

    const isUsefulTipsChanged = () => tipsEmail !== tipsSubscription;

    const isDailyFeedChanged = () =>
      dailyFeedEmail !== isDailyFeedEmailSubscription;

    if (isUsefulTipsChanged())
      requests.push(changeEmailSubscription(tipsEmail));

    onClosePanel();

    try {
      if (requests.length === 0) return;

      await Promise.all(requests);
      toastr.success("success");
    } catch (e) {
      toastr.success(e);
    }
  };

  return (
    <>
      <Button
        key="SaveButton"
        label={t("Common:SaveButton")}
        size="normal"
        primary
        onClick={onSavingChanges}
        scale
      />
      <Button
        key="CancelButton"
        label={t("Common:CancelButton")}
        size="normal"
        onClick={onClosePanel}
        scale
      />
    </>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;
  const {
    tipsSubscription,
    isDailyFeedEmailSubscription,
    changeEmailSubscription,
  } = targetUserStore;

  return {
    tipsSubscription,
    isDailyFeedEmailSubscription,
    changeEmailSubscription,
  };
})(observer(ButtonsContainer));
