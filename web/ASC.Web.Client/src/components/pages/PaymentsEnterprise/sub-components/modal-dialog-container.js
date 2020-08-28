import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";

import {
  Button,
  utils,
  FileInput,
  Link,
  ModalDialog,
} from "asc-web-components";
import { store, history } from "asc-web-common";

import { setLicense } from "../../../../store/payments/actions";
import { resetLicenseUploaded } from "../../../../store/wizard/actions";
// const { getPortalSettings, setIsLoaded } = store.auth.actions;
const { tablet, mobile } = utils.device;

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
const ModalDialogContainer = ({ t, visibleModal, onCloseModal }) => {
  const header = <StyledHeader>{t("LoadingError")}</StyledHeader>;
  const body = <StyledBody>{t("LicenseError")}</StyledBody>;
  return (
    <ModalDialog
      bodyPadding="0px"
      visible={visibleModal}
      displayType="auto"
      zIndex={310}
      headerContent={header}
      bodyContent={body}
      onClose={onCloseModal}
    />
  );
};

export default ModalDialogContainer;
