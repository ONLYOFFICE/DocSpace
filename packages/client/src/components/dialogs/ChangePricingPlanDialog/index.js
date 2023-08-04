import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { useTranslation, Trans } from "react-i18next";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import ModalDialog from "@docspace/components/modal-dialog";
import { inject, observer } from "mobx-react";
import { getConvertedSize } from "@docspace/common/utils";

const ModalDialogContainer = styled(ModalDialog)`
  .cannot-downgrade-plan {
    margin-bottom: 16px;
  }

  .downgrade-plan-wrapper {
    display: flex;
  }
  .save-or-change {
    margin-top: 16px;
  }
`;

const ChangePricingPlanDialog = ({
  visible,
  onClose,
  addedManagersCount,
  managersCount,
  allowedStorageSizeByQuota,
  usedTotalStorageSizeCount,
}) => {
  const { t, ready } = useTranslation(["DowngradePlanDialog", "Common"]);

  const onCloseModal = () => {
    onClose && onClose();
  };

  const allowedStorageSpace = getConvertedSize(t, allowedStorageSizeByQuota);
  const currentStorageSpace = getConvertedSize(t, usedTotalStorageSizeCount);

  const planUsersLimitations = (
    <Text as="span" fontSize="13px">
      <Trans t={t} i18nKey="PlanUsersLimit" ns="DowngradePlanDialog">
        You wish to downgrade the team to
        <strong>{{ usersCount: managersCount }}</strong>
        admins/power users, and current number of such users in your DocSpace is
        <strong>{{ currentUsersCount: addedManagersCount }}</strong>
      </Trans>
    </Text>
  );

  const storagePlanLimitations = (
    <Text as="span" fontSize="13px">
      <Trans t={t} i18nKey="PlanStorageLimit" ns="DowngradePlanDialog">
        New tariff's limitation is
        <strong>{{ storageValue: allowedStorageSpace }}</strong> of storage, and
        your current used storage is
        <strong>{{ currentStorageValue: currentStorageSpace }}</strong>.
      </Trans>
    </Text>
  );

  return (
    <ModalDialogContainer
      visible={visible}
      onClose={onCloseModal}
      autoMaxHeight
      isLarge
      isLoading={!ready}
    >
      <ModalDialog.Header>
        <Text isBold fontSize="21px">
          {t("ChangePricingPlan")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text fontSize="13px" isBold className="cannot-downgrade-plan">
          {t("CannotChangePlan")}
        </Text>
        {planUsersLimitations}
        <br />
        {storagePlanLimitations}

        <Text fontSize="13px" className="save-or-change">
          {t("SaveOrChange")}
        </Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="ok-button"
          label={t("Common:OKButton")}
          size="normal"
          primary={true}
          onClick={onCloseModal}
          tabIndex={3}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

ChangePricingPlanDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default inject(({ auth, payments }) => {
  const { managersCount, allowedStorageSizeByQuota } = payments;
  const { currentQuotaStore } = auth;
  const { addedManagersCount, usedTotalStorageSizeCount } = currentQuotaStore;
  return {
    managersCount,
    addedManagersCount,
    allowedStorageSizeByQuota,
    usedTotalStorageSizeCount,
  };
})(observer(ChangePricingPlanDialog));
