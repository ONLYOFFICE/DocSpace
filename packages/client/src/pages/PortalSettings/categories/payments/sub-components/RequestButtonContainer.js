import React, { useState } from "react";
import Button from "@docspace/components/button";
import styled, { css } from "styled-components";
import SalesDepartmentRequestDialog from "../../../../../components/dialogs/SalesDepartmentRequestDialog";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;

const RequestButtonContainer = ({ isDisabled, isLoading, t }) => {
  const [isVisibleDialog, setIsVisibleDialog] = useState(false);

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
        label={t("SendRequest")}
        size={"medium"}
        primary
        isDisabled={isLoading || isDisabled}
        onClick={toDoRequest}
        isLoading={isLoading}
      />
    </StyledBody>
  );
};

export default RequestButtonContainer;
