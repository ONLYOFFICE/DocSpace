import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { Button, utils, FileInput } from "asc-web-components";

const { tablet } = utils.device;

const StyledButtonContainer = styled.div`
  background: #edf2f7;
  height: 108px;
  margin-bottom: 17px;
  display: flex;
  .button-payments-enterprise {
    border-radius: 3px;
    padding: 12px 20px;
    display: inline-block;
    background: #2da7db;
    color: white;
    height: 45px;
    font-weight: 600;
    font-size: 16px;
    line-height: 20px;
  }
  .button-buy {
    margin: 32px 14px 32px 32px;
  }

  .button-upload {
    margin: 32px 0px 32px 0px;
  }

  @media ${tablet} {
    width: 600px;
    height: 168px;
    display: block;
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
  @media (max-width: 632px) {
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
  onUploadFileClick = () => this.inputFilesElement;

  render() {
    const { t, buyUrl, onButtonClickBuy, onButtonClickUpload } = this.props;
    return (
      <StyledButtonContainer>
        <Button
          className="button-payments-enterprise button-buy"
          label={t("Buy")}
          value={`${buyUrl}`}
          onClick={onButtonClickBuy}
        />
        <FileInput
          id="UploadLicenseFile"
          type="file"
          className=" button-upload input"
          placeholder={"Upload file"}
          accept=".lic"
          ref={(input) => (this.inputFilesElement = input)}
          onInput={onButtonClickUpload}
          style={{ display: "none" }}
        />

        <Button
          id="trigger_link"
          type="submit"
          className="button-payments-enterprise button-upload"
          label={t("Upload")}
          onClick={() => document.getElementById("UploadLicenseFile").click()}
        />
      </StyledButtonContainer>
    );
  }
}

ButtonContainer.propTypes = {
  buyUrl: PropTypes.string.isRequired,
  t: PropTypes.func.isRequired,
  onButtonClickUpload: PropTypes.func.isRequired,
  onButtonClickBuy: PropTypes.func.isRequired,
};
export default ButtonContainer;
