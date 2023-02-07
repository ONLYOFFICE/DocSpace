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
  .allowed-number-managers,
  .current-number-managers_number,
  .current-characteristics {
    margin-left: 3px;
  }
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

const DowngradePlanDialog = ({
  visible,
  onClose,
  addedManagersCount,
  minAvailableManagersValue,
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

  const allowedManagersCountByQuotaComponent = (
    <Text as="span" fontSize="13px">
      <Trans t={t} i18nKey="AllowedManagerNumber" ns="DowngradePlanDialog">
        Number of managers allowed:
        <strong>
          {managersCount !== minAvailableManagersValue
            ? { valuesRange: minAvailableManagersValue + "-" + managersCount }
            : { valuesRange: minAvailableManagersValue }}
        </strong>
      </Trans>
    </Text>
  );

  const currentAddedManagersCountComponent = (
    <Text as="span" fontSize="13px" className="current-characteristics">
      <Trans t={t} i18nKey="CurrentManagerNumber" ns="DowngradePlanDialog">
        Current number of managers:
        <strong>{{ currentCount: addedManagersCount }}</strong>
      </Trans>
    </Text>
  );

  const allowedStorageSpaceByQuotaComponent = (
    <Text as="span" fontSize="13px">
      <Trans t={t} i18nKey="StorageSpaceSizeAllowed" ns="DowngradePlanDialog">
        Storage space size allowed:
        <strong>{{ size: allowedStorageSpace }}</strong>
      </Trans>
    </Text>
  );

  const currentStorageSpaceByQuotaComponent = (
    <Text as="span" fontSize="13px" className="current-characteristics">
      <Trans t={t} i18nKey="CurrentStorageSpace" ns="DowngradePlanDialog">
        Storage space size allowed:
        <strong>{{ size: currentStorageSpace }}</strong>
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
          {t("DowngradePlan")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text fontSize="13px" isBold className="cannot-downgrade-plan">
          {t("CannotDowngradePlan")} {":"}
        </Text>
        {allowedManagersCountByQuotaComponent}
        {currentAddedManagersCountComponent}
        <br />
        {allowedStorageSpaceByQuotaComponent}
        {currentStorageSpaceByQuotaComponent}

        <Text fontSize="13px" className="save-or-change">
          {t("SaveOrChange")}
        </Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          label={t("Common:OKButton")}
          size="normal"
          primary={true}
          onClick={onCloseModal}
          tabIndex={3}
        />
        <Button
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onCloseModal}
          tabIndex={3}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

DowngradePlanDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default inject(({ auth, payments }) => {
  const {
    minAvailableManagersValue,
    managersCount,
    allowedStorageSizeByQuota,
  } = payments;
  const { currentQuotaStore } = auth;
  const { addedManagersCount, usedTotalStorageSizeCount } = currentQuotaStore;
  return {
    minAvailableManagersValue,
    managersCount,
    addedManagersCount,
    allowedStorageSizeByQuota,
    usedTotalStorageSizeCount,
  };
})(observer(DowngradePlanDialog));
