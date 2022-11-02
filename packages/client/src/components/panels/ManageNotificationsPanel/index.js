import { inject, observer } from "mobx-react";
import React, { useEffect, useState, useCallback } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import TelegramConnectionContainer from "./sub-components/TelegramConnectionContainer";
import RoomsActionsContainer from "./sub-components/RoomsActionsContainer";
import DailyFeedContainer from "./sub-components/DailyFeedContainer";
import UsefulTipsContainer from "./sub-components/UsefulTipsContainer";
import ButtonsContainer from "./sub-components/ButtonsContainer";
import { toastr } from "@docspace/components";
import ModalDialogContainer from "./StyledComponent";

const ManageNotificationsPanel = ({
  t,
  isPanelVisible,
  onClosePanel,
  getTipsSubscription,
  isTelegramConnectionAvailable,
}) => {
  useEffect(async () => {
    try {
      const tipsEmail = await getTipsSubscription();

      setSubscription((prevState) => ({
        ...prevState,
        tipsEmail,
      }));
    } catch (e) {
      toastr.error(e);
    }
  }, []);

  const [subscriptions, setSubscription] = useState({
    tipsEmail: false,
    dailyFeedEmail: false,
    dailyFeedTelegram: false,
    roomsActionsBadge: false,
    roomsActionsEmail: false,
    roomsActionsTelegram: false,
  });

  const {
    dailyFeedEmail,
    tipsEmail,
    dailyFeedTelegram,
    roomsActionsBadge,
    roomsActionsEmail,
    roomsActionsTelegram,
  } = subscriptions;

  const onChangeTipsEmailState = useCallback(
    (enable) => {
      setSubscription((prevState) => ({
        ...prevState,
        tipsEmail: enable,
      }));
    },
    [tipsEmail]
  );

  const onChangeDailyFeedEmailState = useCallback(
    (enable) => {
      setSubscription((prevState) => ({
        ...prevState,
        dailyFeedEmail: enable,
      }));
    },
    [dailyFeedEmail]
  );

  const onChangeDailyFeedTelegramSubscription = useCallback(
    (enable) => {
      setSubscription((prevState) => ({
        ...prevState,
        dailyFeedTelegram: enable,
      }));
    },
    [dailyFeedTelegram]
  );

  const onChangeRoomsActionsBadgeSubscription = useCallback(
    (enable) => {
      setSubscription((prevState) => ({
        ...prevState,
        roomsActionsBadge: enable,
      }));
    },
    [roomsActionsBadge]
  );

  const onChangeRoomsActionsEmailSubscription = useCallback(
    (enable) => {
      setSubscription((prevState) => ({
        ...prevState,
        roomsActionsEmail: enable,
      }));
    },
    [roomsActionsEmail]
  );

  const onChangeRoomsActionsTelegramSubscription = useCallback(
    (enable) => {
      setSubscription((prevState) => ({
        ...prevState,
        roomsActionsTelegram: enable,
      }));
    },
    [roomsActionsTelegram]
  );

  return (
    <ModalDialogContainer
      visible={isPanelVisible}
      onClose={onClosePanel}
      displayType="aside"
      withoutBodyScroll
      withFooterBorder
    >
      <ModalDialog.Header>{t("ManageNotifications")}</ModalDialog.Header>
      <ModalDialog.Body>
        {isTelegramConnectionAvailable && <TelegramConnectionContainer t={t} />}
        <RoomsActionsContainer
          t={t}
          isEnableEmail={roomsActionsEmail}
          isEnableBadge={roomsActionsBadge}
          isEnableTelegram={roomsActionsTelegram}
          onChangeBadgeState={onChangeRoomsActionsBadgeSubscription}
          onChangeEmailState={onChangeRoomsActionsEmailSubscription}
          onChangeTelegramState={onChangeRoomsActionsTelegramSubscription}
          isTelegramConnectionAvailable={isTelegramConnectionAvailable}
        />
        <DailyFeedContainer
          t={t}
          onChangeEmailState={onChangeDailyFeedEmailState}
          onChangeTelegramState={onChangeDailyFeedTelegramSubscription}
          isEnableEmail={dailyFeedEmail}
          isEnableTelegram={dailyFeedTelegram}
          isTelegramConnectionAvailable={isTelegramConnectionAvailable}
        />
        <UsefulTipsContainer
          t={t}
          onChangeTipsEmailState={onChangeTipsEmailState}
          isEnableEmail={tipsEmail}
        />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <ButtonsContainer
          t={t}
          onClosePanel={onClosePanel}
          dailyFeedEmail={dailyFeedEmail}
          tipsEmail={tipsEmail}
          roomsActionsBadge={roomsActionsBadge}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;
  const { getTipsSubscription } = targetUserStore;

  const isTelegramConnectionAvailable = true;

  return {
    getTipsSubscription,
    isTelegramConnectionAvailable,
  };
})(observer(ManageNotificationsPanel));
