import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Button, utils } from "asc-web-components";
import { store, history } from "asc-web-common";
const { getPortalSettings, setIsLoaded } = store.auth.actions;
const { tablet, mobile } = utils.device;
const onButtonClickBuy = (e) => {
  getPortalSettings(store.dispatch)
    .then(() => store.dispatch(setIsLoaded(true)))
    .catch((e) => history.push(`/login/error=${e}`));
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

const ButtonContainer = ({ t }) => {
  return (
    <StyledButtonContainer>
      <Button
        className="button-payments-enterprise button-buy"
        label="Buy now"
        onClick={onButtonClickBuy}
      />
      <Button
        className="button-payments-enterprise button-upload"
        label="Upload license"
      />
    </StyledButtonContainer>
  );
};

ButtonContainer.propTypes = {
  t: PropTypes.func.isRequired,
};

export default ButtonContainer;
