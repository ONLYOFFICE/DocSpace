import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Button, utils, FileInput, Link } from "asc-web-components";
import { store, history } from "asc-web-common";

import { setLicense } from "../../../../store/payments/actions";
import { resetLicenseUploaded } from "../../../../store/wizard/actions";
// const { getPortalSettings, setIsLoaded } = store.auth.actions;
const { tablet, mobile } = utils.device;

const StyledButtonContainer = styled.div`
  position: static;
  background: #edf2f7;
  height: 108px;
  margin-bottom: 17px;
  position: relative;
  .button-payments-enterprise {
    border-radius: 3px;
    padding: 13px 20px;
    padding: 0px;
    background: #2da7db;
    color: white;
    height: 44px;
    font-weight: 600;
    font-size: 16px;
    line-height: 20px;
  }
  .button-buy {
    width: 107px;
    margin: 32px 16px 32px 32px;
  }
  .button-upload {
    width: 153px;
    margin: 32px 612px 32px 0px;
  }

  .input {
    position: absolute;

    left: 155px; /*width of button-buy and margin*/
    bottom: 0px;
    opacity: 0;
    z-index: 1;
  }
  @media ${tablet} {
    width: 600px;
    height: 168px;

    .button-buy {
      width: 536px;

      margin: 32px 32px 16px 32px;
      border-radius: 3px;
    }
    .button-upload {
      width: 536px;
      margin: 0px 32px 32px 32px;

      border-radius: 3px;
    }
    .input {
      bottom: 0px;
      left: 0px;
    }
  }
  @media ${mobile} {
    width: 343px;
    height: 168px;
    .button-buy {
      width: 279px;
    }
    .button-upload {
      width: 279px;
    }
  }
`;

class ButtonContainer extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      errorMessage: null,
      errorLoading: false,
      hasErrorLicense: false,
    };
  }
  onButtonClickUpload = (file) => {
    //const { wizardToken } = this.props;

    const { licenseUpload, setLicense } = this.props;
    //if (licenseUpload) resetLicenseUploaded();
    let fd = new FormData();
    fd.append("files", file);

    setLicense(null, fd).catch((e) =>
      this.setState({
        errorLoading: true,
        errorMessage: e,
        hasErrorLicense: true,
      })
    );
  };
  onButtonClickBuy = (e) => {
    window.open(e.target.value, "_blank");
  };
  render() {
    const { t, buyUrl } = this.props;
    const { errorLoading, hasErrorLicense } = this.state;

    return (
      <StyledButtonContainer>
        <Button
          className="button-payments-enterprise button-buy"
          label={t("Buy")}
          value={`${buyUrl}`}
          onClick={this.onButtonClickBuy}
        />
        <FileInput
          type="file"
          className="button-payments-enterprise button-upload input"
          placeholder={"Upload file"}
          accept=".lic"
          onInput={this.onButtonClickUpload}
        />
        <Button
          type="submit"
          className="button-payments-enterprise button-upload"
          label={t("Upload")}
        />
      </StyledButtonContainer>
    );
  }
}

function mapStateToProps(state) {
  return {
    buyUrl: state.payments.buyUrl,
    licenseUpload: state.payments.licenseUpload,
  };
}

export default connect(mapStateToProps, { setLicense, resetLicenseUploaded })(
  withRouter(ButtonContainer)
);
