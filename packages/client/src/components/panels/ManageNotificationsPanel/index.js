import { inject, observer } from "mobx-react";
import React, { useEffect, useState, useCallback } from "react";
import { useTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import styled from "styled-components";
import TelegramConnectionContainer from "./sub-components/TelegramConnectionContainer";
import RoomsActionsContainer from "./sub-components/RoomsActionsContainer";
import DailyFeedContainer from "./sub-components/DailyFeedContainer";
import UsefulTipsContainer from "./sub-components/UsefulTipsContainer";
import ButtonsContainer from "./sub-components/ButtonsContainer";
import { toastr } from "@docspace/components";

const ModalDialogContainer = styled(ModalDialog)`
  .toggle-btn {
    position: relative;
    align-items: center;
    height: 20px;
    grid-gap: 12px !important;
  }
  .toggle-btn_next {
    margin-top: 12px;
  }
  button {
    max-width: fit-content;
  }
  .subscription-container {
    margin-bottom: 24px;
    .subscription-title {
      margin-bottom: 12px;
    }
  }
`;
const ManageNotificationsPanel = ({
  t,
  isPanelVisible,
  onClosePanel,
  getTipsSubscription,
  getTelegramConnection,
  getDailyFeedEmailSubscription,
}) => {
  useEffect(async () => {
    const requests = [
      getTelegramConnection(),
      getTipsSubscription(),
      getDailyFeedEmailSubscription(),
    ];

    try {
      const [, tipsEmail, dailyFeedEmail] = await Promise.all(requests);

      setSubscription((prevState) => ({
        ...prevState,
        tipsEmail,
        dailyFeedEmail,
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
  });

  const {
    dailyFeedEmail,
    tipsEmail,
    dailyFeedTelegram,
    roomsActionsBadge,
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
    [dailyFeedTelegram]
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
        {/* <TelegramConnectionContainer t={t} /> */}
        <RoomsActionsContainer
          t={t}
          isEnableBadge={roomsActionsBadge}
          onChangeBadgeSubscription={onChangeRoomsActionsBadgeSubscription}
        />
        <DailyFeedContainer
          t={t}
          onChangeEmailState={onChangeDailyFeedEmailState}
          onChangeTelegramState={onChangeDailyFeedTelegramSubscription}
          isEnableEmail={dailyFeedEmail}
          isEnableTelegram={dailyFeedTelegram}
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
  const {
    getTipsSubscription,
    getTelegramConnection,
    getDailyFeedEmailSubscription,
  } = targetUserStore;
  return {
    getTipsSubscription,
    getTelegramConnection,
    getDailyFeedEmailSubscription,
  };
})(observer(ManageNotificationsPanel));
