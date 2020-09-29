import React, { useEffect, createRef } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Button, utils as Utils, toastr } from "asc-web-components";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../../helpers/i18n";
import { utils } from "asc-web-common";
import { setPaymentsLicense } from "../../../../store/payments/actions";
const { changeLanguage } = utils;
const { tablet } = Utils.device;
const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});

const StyledButtonContainer = styled.div`
  background: #edf2f7;
  margin-bottom: 16px;
  display: grid;
  padding: 32px;
  grid-template-columns: min-content min-content;
  grid-template-rows: min-content;
  grid-column-gap: 16px;

  .input-upload {
    display: none;
  }
  @media ${tablet} {
    grid-template-columns: 1fr;
    grid-template-rows: min-content min-content;
    grid-row-gap: 16px;
  }
`;

class Body extends React.PureComponent {
  constructor(props) {
    super(props);
    this.inputFilesElementRef = createRef(null);
  }
  onClickSubmit = () => {
    this.inputFilesElementRef &&
      this.inputFilesElementRef.current &&
      this.inputFilesElementRef.current.click();
  };

  onClickUpload = (e) => {
    const { setPaymentsLicense, t } = this.props;

    let fd = new FormData();
    fd.append("files", e.currentTarget.files[0]);

    setPaymentsLicense(null, fd)
      .then(() => {
        toastr.success(t("LoadingLicenseSuccess"), "");
      })
      .catch((error) => {
        toastr.error(t("LoadingLicenseError"), "LicenseIsNotValid", 0, true);
        console.log(error);
      });
  };

  onClickBuy = (e) => {
    window.open(e.target.value, "_blank");
  };

  render() {
    const { t } = this.props;
    const { buyUrl } = this.props;

    return (
      <StyledButtonContainer>
        <Button
          label={t("ButtonBuyLicense")}
          value={`${buyUrl}`}
          size="large"
          primary={true}
          onClick={this.onClickBuy}
        />
        <input
          type="file"
          className="input-upload"
          accept=".lic"
          ref={this.inputFilesElementRef}
          onInput={this.onClickUpload}
        />

        <Button
          type="submit"
          label={t("ButtonUploadLicense")}
          size="large"
          primary={true}
          onClick={this.onClickSubmit}
        />
      </StyledButtonContainer>
    );
  }
}

const ButtonWrapper = withTranslation()(Body);
const ButtonContainer = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return (
    <I18nextProvider i18n={i18n}>
      <ButtonWrapper {...props} />
    </I18nextProvider>
  );
};

ButtonContainer.propTypes = {
  buyUrl: PropTypes.string,
};
function mapStateToProps(state) {
  const { buyUrl } = state.payments;
  return {
    buyUrl,
  };
}
export default connect(mapStateToProps, { setPaymentsLicense })(
  ButtonContainer
);
