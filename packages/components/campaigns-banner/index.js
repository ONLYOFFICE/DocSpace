import React, { useState } from "react";
import PropTypes from "prop-types";
import BannerWrapper from "./styled-campaigns-banner";

import Button from "../button";
import Text from "../text";
import Loaders from "@docspace/common/components/Loaders";

const onButtonClick = (url) => {
  window.open(url, "_blank");
};

const CampaignsBanner = (props) => {
  const { headerLabel, subHeaderLabel, img, buttonLabel, link } = props;
  const [imageLoad, setImageLoad] = useState(false);

  const handleImageLoaded = () => {
    setImageLoad(true);
  };

  const onMouseDown = (e) => {
    e.preventDefault();
  };

  return (
    <BannerWrapper>
      <a href={link} target="_blank" rel="noreferrer">
        <Text noSelect fontWeight="700" fontSize="13px">
          {headerLabel}
        </Text>
        <Text
          noSelect
          className="banner-sub-header"
          fontWeight="500"
          fontSize="12px"
        >
          {subHeaderLabel}
        </Text>
        <img
          className="banner-img"
          src={img}
          onMouseDown={onMouseDown}
          onLoad={handleImageLoaded}
        />
        {!imageLoad && <Loaders.Rectangle height="140px" borderRadius="5px" />}
      </a>

      <Button
        className="banner-btn"
        primary
        size="small"
        isDisabled={false}
        disableHover={true}
        label={buttonLabel}
        onClick={() => onButtonClick(link)}
      />
    </BannerWrapper>
  );
};

CampaignsBanner.propTypes = {
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.object,
  /** Label */
  headerLabel: PropTypes.string,
  /** Label subheader */
  subHeaderLabel: PropTypes.string,
  /** Image source */
  img: PropTypes.string,
  /** Header button text */
  buttonLabel: PropTypes.string,
  /** The link that opens when the button is clicked */
  link: PropTypes.string,
};

CampaignsBanner.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default CampaignsBanner;
