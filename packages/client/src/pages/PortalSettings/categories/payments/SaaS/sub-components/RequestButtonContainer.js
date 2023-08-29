import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import Button from "@docspace/components/button";
import styled, { css } from "styled-components";
import SalesDepartmentRequestDialog from "../../../../../../components/dialogs/SalesDepartmentRequestDialog";
import { inject, observer } from "mobx-react";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;

const RequestButtonContainer = ({ isDisabled, isLoading }) => {
  const [isVisibleDialog, setIsVisibleDialog] = useState(false);
  const { t } = useTranslation(["Common"]);

  const toDoRequest = () => {
    setIsVisibleDialog(true);
  };

  const onClose = () => {
    isVisibleDialog && setIsVisibleDialog(false);
  };

  return (
    <StyledBody>
      {isVisibleDialog && (
        <SalesDepartmentRequestDialog
          visible={isVisibleDialog}
          onClose={onClose}
        />
      )}
      <Button
        className="send-request-button"
        label={t("Common:SendRequest")}
        size={"medium"}
        primary
        isDisabled={isLoading || isDisabled}
        onClick={toDoRequest}
        isLoading={isLoading}
      />
    </StyledBody>
  );
};

export default inject(({ payments }) => {
  const { isLoading } = payments;

  return {
    isLoading,
  };
})(observer(RequestButtonContainer));
