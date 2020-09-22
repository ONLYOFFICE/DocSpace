import React, { useRef, useCallback, useEffect } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Button, utils as Utils } from "asc-web-components";
import { useTranslation } from "react-i18next";
import { createI18N } from "../../../../helpers/i18n";
import { utils } from "asc-web-common";
const { changeLanguage } = utils;
const { tablet } = Utils.device;
const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});

const StyledButtonContainer = styled.div`
  background: #edf2f7;
  height: 108px;
  margin-bottom: 18px;
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
  .input-upload {
    display: none;
  }
  @media ${tablet} {
    height: 168px;
    display: grid;
    .button-buy {
      max-width: 536px;

      margin: 32px 32px 16px 32px;
      border-radius: 3px;
    }
    .button-upload {
      max-width: 536px;
      margin: 0px 32px 32px 32px;

      border-radius: 3px;
    }
  }
  @media (max-width: 632px) {
    height: 168px;
    margin-bottom: 16px;
    .button-buy {
      min-width: 279px;
    }
    .button-upload {
      min-width: 279px;
    }
  }
`;

const ButtonContainer = ({ buyUrl, onClickBuy, onClickUpload }) => {
  const inputFilesElementRef = useRef(null);

  const onClickSubmit = useCallback(() => {
    inputFilesElementRef &&
      inputFilesElementRef.current &&
      inputFilesElementRef.current.click();
  }, [inputFilesElementRef]);

  const onInput = useCallback(
    (e) => {
      onClickUpload && onClickUpload(e.currentTarget.files[0]);
    },
    [onClickUpload]
  );
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  const { t } = useTranslation("translation", { i18n });
  return (
    <StyledButtonContainer>
      <Button
        className="button-payments-enterprise button-buy"
        label={t("Buy")}
        value={`${buyUrl}`}
        onClick={onClickBuy}
      />
      <input
        type="file"
        className="input-upload"
        accept=".lic"
        ref={inputFilesElementRef}
        onInput={onInput}
      />

      <Button
        type="submit"
        className="button-payments-enterprise button-upload"
        label={t("Upload")}
        onClick={onClickSubmit}
      />
    </StyledButtonContainer>
  );
};

ButtonContainer.propTypes = {
  buyUrl: PropTypes.string,
  onClickUpload: PropTypes.func.isRequired,
  onClickBuy: PropTypes.func.isRequired,
};
function mapStateToProps(state) {
  return {
    buyUrl: state.payments.buyUrl,
  };
}
export default connect(mapStateToProps)(withRouter(ButtonContainer));
