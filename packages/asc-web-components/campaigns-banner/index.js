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
  const { headerLabel, subHeaderLabel, img, btnLabel, link } = props;
  return (
    <BannerWrapper>
      <a href={link}>
        <BannerHeader>{headerLabel}</BannerHeader>
        <BannerText>{subHeaderLabel}</BannerText>
        <img src={img} />
      </a>

      <Button
        className="banner-btn"
        size="big"
        isDisabled={false}
        disableHover={true}
        label={btnLabel}
        onClick={() => onButtonClick(link)}
      />
    </BannerWrapper>
  );
};

CampaignsBanner.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  headerLabel: PropTypes.string,
  subHeaderLabel: PropTypes.string,
  img: PropTypes.string,
  btnLabel: PropTypes.string,
  link: PropTypes.string,
};

CampaignsBanner.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default CampaignsBanner;
