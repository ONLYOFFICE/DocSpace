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

const onButtonClickBuy = (e) => {
  window.open(e.target.value, "_blank");
};

const StyledButtonContainer = styled.div`
  position: static;
  background: #edf2f7;
  height: 108px;
  margin-bottom: 17px;

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
    border: 2px solid red;
    margin-right: 40px;
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
  }
  @media ${mobile} {
    width: 343px;
    height: 168px;
    .button-buy {
      width: 279px;

      margin: 32px 32px 16px 32px;
      border-radius: 3px;
    }
    .button-upload {
      width: 279px;
      margin: 0px 32px 32px 32px;

      border-radius: 3px;
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
  onInputFileHandler = (file) => {
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
  render() {
    const { t, buyUrl } = this.props;
    const { errorLoading, hasErrorLicense } = this.state;

    return (
      <StyledButtonContainer>
        <Button
          className="button-payments-enterprise button-buy"
          label={t("Buy")}
          value={`${buyUrl}`}
          onClick={onButtonClickBuy}
        />
        {/* <Link
          type="action"
          color="black"
          isBold={true}
          onClick={this.onInputFileHandler}
        >
          {t("Upload")}
        </Link> */}
        {/* <Button
          type="submit"
          className="button-payments-enterprise button-upload"
          label={t("Upload")}
          onCLick={this.onInputFileHandler}
        /> */}

        <FileInput
          tabIndex={3}
          className="input"
          placeholder={"Upload file"}
          accept=".lic"
          onInput={this.onInputFileHandler}
        />
      </StyledButtonContainer>
    );
  }
}

function mapStateToProps(state) {
  return {
    buyUrl: state.payments.buyUrl,
    wizardToken: state.payments.wizardToken,
    licenseUpload: state.payments.licenseUpload,
  };
}

export default connect(mapStateToProps, { setLicense, resetLicenseUploaded })(
  withRouter(ButtonContainer)
);
