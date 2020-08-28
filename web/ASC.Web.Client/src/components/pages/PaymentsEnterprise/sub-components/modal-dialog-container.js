import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { utils, ModalDialog } from "asc-web-components";
const { tablet } = utils.device;
const StyledBody = styled.div`
  font-family: Open Sans;
  font-style: normal;
  font-weight: normal;
  font-size: 13px;
  line-height: 20px;
  margin-top: 16px;
  @media ${tablet} {
    margin-right: 10px;
  }
`;
const StyledHeader = styled.div`
  font-family: Open Sans;
  font-style: normal;
  font-weight: bold;
  font-size: 21px;
  line-height: 16px;
  margin-bottom: 16px;
  margin-top: 16px;

  @media ${tablet} {
    margin-bottom: 17px;
  }
`;
const ModalDialogContainer = ({
  t,
  isVisibleModalDialog,
  onCloseModalDialog,
}) => {
  const header = <StyledHeader>{t("LoadingError")}</StyledHeader>;
  const body = <StyledBody>{t("LicenseError")}</StyledBody>;
  return (
    <ModalDialog
      bodyPadding="0px"
      visible={isVisibleModalDialog}
      displayType="auto"
      zIndex={310}
      headerContent={header}
      bodyContent={body}
      onClose={onCloseModalDialog}
    />
  );
};
ModalDialogContainer.propTypes = {
  isVisibleModalDialog: PropTypes.bool.isRequired,
  onCloseModalDialog: PropTypes.bool.isRequired,
  t: PropTypes.func.isRequired,
};
export default ModalDialogContainer;
