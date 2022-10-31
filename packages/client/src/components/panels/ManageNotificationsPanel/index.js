import { inject, observer } from "mobx-react";
import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import styled from "styled-components";
import TelegramConnectionContainer from "./sub-components/TelegramConnectionContainer";
import RoomsActionsContainer from "./sub-components/RoomsActionsContainer";
import DailyFeedContainer from "./sub-components/DailyFeedContainer";
import UsefulTipsContainer from "./sub-components/UsefulTipsContainer";
import ButtonsContainer from "./sub-components/ButtonsContainer";

const ModalDialogContainer = styled(ModalDialog)``;
const ManageNotificationsPanel = ({
  t,
  isPanelVisible,
  onClosePanel,
  getTipsSubscription,
}) => {
  useEffect(async () => {
    const requests = [getTipsSubscription()];

    await Promise.all(requests);
  }, []);

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
        <TelegramConnectionContainer t={t} />
        <RoomsActionsContainer t={t} />
        <DailyFeedContainer t={t} />
        <UsefulTipsContainer t={t} />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <ButtonsContainer t={t} onClosePanel={onClosePanel} />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;
  const { getTipsSubscription } = targetUserStore;
  return { getTipsSubscription };
})(observer(ManageNotificationsPanel));
