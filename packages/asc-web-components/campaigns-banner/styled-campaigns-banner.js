import styled from "styled-components";
import Base from "../themes/base";
import NoUserSelect from "../utils/commonStyles";

const BannerWrapper = styled.div`
  max-width: 185px;
  border: ${(props) => props.theme.campaignsBanner.border};
  border-radius: 5px;
  padding: 15px;
  margin: 20px 0px 50px 0px;

  @media screen and (max-width: 1024px) {
    max-width: inherit;
  }

  a {
    text-decoration: none;
    color: ${(props) => props.theme.campaignsBanner.color};
  }

  .banner-img-wrapper {
    height: 160px;
    width: 100%;
  }

  img {
    max-width: 100%;
    height: auto;
    margin-top: 5px;
  }

  .banner-sub-header {
    line-height: 1.5;
  }

  .banner-img {
    ${NoUserSelect}
  }

  .banner-btn {
    width: 100%;
    color: ${(props) => props.theme.campaignsBanner.btnColor};
    margin-top: 15px;
    border: none;
    border-radius: 5px;
  }

  .banner-btn:active {
    color: ${(props) => props.theme.campaignsBanner.btnColor};
    border: none;
  }
`;

BannerWrapper.defaultProps = { theme: Base };

export default BannerWrapper;
