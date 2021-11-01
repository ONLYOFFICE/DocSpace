import React, { useState } from "react";
import PropTypes from "prop-types";
import BannerWrapper from "./styled-campaigns-banner";

import Button from "../button";
import Text from "../text";
import Loaders from "@appserver/common/components/Loaders";

const onButtonClick = (url) => {
  window.location = url;
};

const CampaignsBanner = (props) => {
  const { headerLabel, subHeaderLabel, img, btnLabel, link } = props;
  const [imageLoad, setImageLoad] = useState(false);

  const handleImageLoaded = () => {
    setImageLoad(true);
  };

  const onMouseDown = (e) => {
    e.preventDefault();
  };

  return (
    <BannerWrapper>
      <a href={link}>
        <Text fontWeight="700" fontSize="13px">
          {headerLabel}
        </Text>
        <Text className="banner-sub-header" fontWeight="500" fontSize="12px">
          {subHeaderLabel}
        </Text>

        {!imageLoad && <Loaders.Rectangle height="140px" borderRadius="5px" />}
        <img src={img} onMouseDown={onMouseDown} onLoad={handleImageLoaded} />
      </a>

      <Button
        className="banner-btn"
        primary
        size="medium"
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
