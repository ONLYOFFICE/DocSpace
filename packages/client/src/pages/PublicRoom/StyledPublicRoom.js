import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, mobile } from "@docspace/components/utils/device";
import Headline from "@docspace/common/components/Headline";

const StyledHeadline = styled(Headline)`
  font-weight: 700;
  font-size: ${isMobile ? "21px !important" : "18px"};
  line-height: ${isMobile ? "28px !important" : "24px"};
  margin-right: 18px;

  @media ${tablet} {
    font-size: 21px;
    line-height: 28px;
  }
`;

const StyledContainer = styled.div`
  width: 100%;
  height: 32px;
  display: flex;

  align-items: center;

  .public-room-header_separator {
    border-left: 1px solid #dfe2e3;
    margin: 0 16px 0 15px;
    height: 21px;
  }

  @media ${tablet} {
    width: 100%;
    padding: 16px 0 0px;
  }

  ${isMobile &&
  css`
    width: 100% !important;
    padding: 16px 0 0px;
  `}

  @media ${mobile} {
    width: 100%;
    padding: 12px 0 0;
  }

  ${isMobileOnly &&
  css`
    width: 100% !important;
    padding: 12px 0 0;
  `}
`;

export { StyledHeadline, StyledContainer };
