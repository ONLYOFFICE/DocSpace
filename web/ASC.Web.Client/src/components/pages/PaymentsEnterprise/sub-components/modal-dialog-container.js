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

const StyledModalDialog = styled.div``;

const ModalDialogContainer = ({
  t,
  buyUrl,
  onButtonClickBuy,
  onButtonClickUpload,
}) => {
  return <StyledModalDialog></StyledModalDialog>;
};

export default ModalDialogContainer;
