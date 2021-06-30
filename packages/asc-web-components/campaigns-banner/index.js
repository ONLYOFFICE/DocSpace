import React from "react";
import PropTypes from "prop-types";
import {
  BannerWrapper,
  BannerHeader,
  BannerText,
} from "./styled-campaigns-banner";

import Button from "../button";

const onButtonClick = (url) => {
  window.location = url;
};

const CampaignsBanner = (props) => {
  const { headerLabel, textLabel, img, btnLabel, btnLink } = props;
  return (
    <BannerWrapper>
      <BannerHeader>{headerLabel}</BannerHeader>
      <BannerText>{textLabel}</BannerText>

      <div>
        <img src={img} />
      </div>

      <Button
        className="banner-btn"
        size="big"
        isDisabled={false}
        label={btnLabel}
        onClick={() => onButtonClick(btnLink)}
      />
    </BannerWrapper>
  );
};

CampaignsBanner.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  headerLabel: PropTypes.string,
  textLabel: PropTypes.string,
  img: PropTypes.string,
  btnLabel: PropTypes.string,
  btnLink: PropTypes.string,
};

CampaignsBanner.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default CampaignsBanner;
