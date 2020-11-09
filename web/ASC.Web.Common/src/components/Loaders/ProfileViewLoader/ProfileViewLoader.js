import React from "react";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";
import CircleLoader from "../CircleLoader/index";
import PropTypes from "prop-types";

import { utils } from "asc-web-components";

const { desktop, mobile } = utils.device;
const { isSmallTablet, isTablet, isMobile } = utils.device;

const StyledBox1 = styled.div`
  display: grid;
  grid-template-columns: 160px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: 32px;

  @media ${mobile} {
    grid-template-columns: 1fr;
    grid-template-rows: 1fr 1fr;
  }
  margin-bottom: 12px;
`;

const StyledBox2 = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 160px 36px;
  grid-row-gap: 12px;

  padding-bottom: 40px;

  @media ${mobile} {
    padding-bottom: 32px;
  }
`;

const StyledBox3 = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(9, 1fr);
  grid-row-gap: 8px;
  padding-bottom: 40px;
`;

const StyledBox4 = styled.div`
  display: grid;
  grid-template-columns: repeat(2, 200px);
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  padding-top: 40px;
  padding-bottom: 40px;
  @media ${desktop} {
    grid-template-columns: repeat(3, 200px);
  }
  @media ${mobile} {
    grid-template-columns: 200px;
  }
`;

const StyledSpacer = styled.div`
  padding-bottom: 40px;
`;

const ProfileViewLoader = (props) => {
  return (
    <div>
      <StyledBox1>
        <StyledBox2>
          <CircleLoader x="80" y="80" radius="80" {...props} />
          {props.isEditBtn ? (
            <RectangleLoader width="160" height="36" {...props} />
          ) : (
            <></>
          )}
        </StyledBox2>
        <StyledBox3>
          <RectangleLoader width="231" height="16" {...props} />
          <RectangleLoader width="231" height="16" {...props} />
          <RectangleLoader width="231" height="16" {...props} />
          <RectangleLoader width="231" height="16" {...props} />
          <RectangleLoader width="231" height="16" {...props} />
          <RectangleLoader width="231" height="16" {...props} />
          <RectangleLoader width="231" height="16" {...props} />
          <RectangleLoader width="231" height="16" {...props} />
          <RectangleLoader width="111" height="16" {...props} />
        </StyledBox3>
        <RectangleLoader width="200" height="24" {...props} />
      </StyledBox1>
      <RectangleLoader width="100%" height="80" {...props} />
      <StyledSpacer />

      <RectangleLoader width="200" height="24" {...props} />
      <StyledBox4>
        {isMobile() || isSmallTablet() ? (
          <RectangleLoader width="200" height="80" {...props} />
        ) : isTablet() ? (
          <>
            <RectangleLoader width="200" height="80" {...props} />
            <RectangleLoader width="200" height="80" {...props} />
          </>
        ) : (
          <>
            <RectangleLoader width="200" height="80" {...props} />
            <RectangleLoader width="200" height="80" {...props} />{" "}
            <RectangleLoader width="200" height="80" {...props} />
          </>
        )}
      </StyledBox4>

      <RectangleLoader width="200" height="24" {...props} />
      <StyledBox4>
        {isMobile() || isSmallTablet() ? (
          <RectangleLoader width="200" height="80" {...props} />
        ) : (
          <>
            <RectangleLoader width="200" height="80" {...props} />
            <RectangleLoader width="200" height="80" {...props} />
          </>
        )}
      </StyledBox4>
    </div>
  );
};

ProfileViewLoader.propTypes = {
  isEditBtn: PropTypes.bool,
};

ProfileViewLoader.defaultProps = {
  isEditBtn: true,
};

export default ProfileViewLoader;
