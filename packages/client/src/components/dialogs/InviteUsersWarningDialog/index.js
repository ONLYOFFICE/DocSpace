import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation, Trans } from "react-i18next";
import { withRouter } from "react-router";
import moment from "moment";
import { combineUrl } from "@docspace/common/utils";
import AppServerConfig from "@docspace/common/constants/AppServerConfig";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";

const PROXY_BASE_URL = combineUrl(AppServerConfig.proxyURL, "/portal-settings");

const InviteUsersWarningDialog = (props) => {
  const {
    t,
    tReady,
    history,
    language,
    dueDate,
    delayDueDate,
    visible,
    setIsVisible,
    isGracePeriod,
  } = props;

  const [datesData, setDatesData] = useState({});

  const { fromDate, byDate, delayDaysCount } = datesData;

  useEffect(() => {
    moment.locale(language);

    gracePeriodDays();
  }, [language, gracePeriodDays]);

  const gracePeriodDays = () => {
    const fromDateMoment = moment(dueDate);
    const byDateMoment = moment(delayDueDate);

    setDatesData({
      fromDate: fromDateMoment.format("LL"),
      byDate: byDateMoment.format("LL"),
      delayDaysCount: fromDateMoment.to(byDateMoment, true),
    });
  };

  const onClose = () => setIsVisible(false);

  const onUpgradePlan = () => {
    onClose();

    const paymentPageUrl = combineUrl(
      PROXY_BASE_URL,
      "/payments/portal-payments"
    );
    history.push(paymentPageUrl);
  };

  return (
    <ModalDialog
      isLarge={isGracePeriod}
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("Common:Warning")}</ModalDialog.Header>
      <ModalDialog.Body>
        {isGracePeriod ? (
          <>
            <Text fontWeight={700} noSelect>
              {t("BusinessPlanPaymentOverdue")}
            </Text>
            <br />
            <Text noSelect as="div">
              <Trans t={t} i18nKey="GracePeriodActivatedInfo" ns="Payments">
                Grace period activated from
                <strong>
                  {{ fromDate }} to {{ byDate }}
                </strong>
                ({{ delayDaysCount }})
              </Trans>
            </Text>
            <br />
            <Text>{t("GracePeriodActivatedDescription")}</Text>
          </>
        ) : (
          <>
            <Text fontWeight={700} noSelect>
              {t("PaymentOverdue")}
            </Text>
            <br />
            <Text>{t("UpgradePlanInfo")}</Text>
            <br />
            <Text>{t("ChooseNewPlan")}</Text>
          </>
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={t("UpgradePlan")}
          size="normal"
          primary
          onClick={onUpgradePlan}
          scale
        />
        <Button
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
          scale
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ auth, dialogsStore }) => {
  const {
    dueDate,
    delayDueDate,
    isGracePeriod,
  } = auth.currentTariffStatusStore;

  const {
    inviteUsersWarningDialogVisible,
    setInviteUsersWarningDialogVisible,
  } = dialogsStore;

  return {
    language: auth.language,
    visible: inviteUsersWarningDialogVisible,
    setIsVisible: setInviteUsersWarningDialogVisible,
    dueDate,
    delayDueDate,
    isGracePeriod,
  };
})(
  observer(
    withTranslation(["Payments", "Common"])(
      withRouter(InviteUsersWarningDialog)
    )
  )
);
