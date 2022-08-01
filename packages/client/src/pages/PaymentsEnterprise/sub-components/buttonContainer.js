import React, { createRef } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import { tablet } from "@docspace/components/utils/device";
import toastr from "@docspace/components/toast/toastr";
import { withRouter } from "react-router";
import { Base } from "@docspace/components/themes";

const StyledButtonContainer = styled.div`
  background: ${(props) =>
    props.theme.client.paymentsEnterprise.buttonBackground};
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

StyledButtonContainer.defaultProps = { theme: Base };

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
        toastr.success(t("SuccessLoadingLicense"), "");
      })
      .catch((error) => {
        toastr.error(t("ErrorLoadingLicense"), "LicenseIsNotValid", 0, true);
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
          size="medium"
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
          size="medium"
          primary={true}
          onClick={this.onClickSubmit}
        />
      </StyledButtonContainer>
    );
  }
}

const ButtonContainer = withTranslation("PaymentsEnterprise")(Body);

ButtonContainer.propTypes = {
  buyUrl: PropTypes.string,
};

export default inject(({ payments }) => {
  const { buyUrl, setPaymentsLicense } = payments;
  return {
    buyUrl,
    setPaymentsLicense,
  };
})(withRouter(observer(ButtonContainer)));
