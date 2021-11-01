import styled from "styled-components";

const BannerWrapper = styled.div`
  max-width: 185px;
  border: 1px solid #d1d1d1;
  border-radius: 5px;
  padding: 15px;
  margin: 20px 0px 50px 0px;

  @media screen and (max-width: 1024px) {
    max-width: inherit;
  }

  a {
    text-decoration: none;
    color: #000;
  }

  img {
    max-width: 100%;
    height: auto;
    margin-top: 10px;
  }

  .banner-sub-header {
    line-height: 1.5;
  }

  .banner-btn {
    width: 100%;
    color: #fff;
    margin-top: 15px;
    border: none;
    border-radius: 5px;
  }

  .banner-btn:active {
    color: #fff;
    background: #2da7db;
    border: none;
  }
`;

export default BannerWrapper;
