import styled from "styled-components";

const BannerWrapper = styled.div`
  width: 238px;
  border: 1px solid #d1d1d1;
  border-radius: 5px;
  padding: 15px;

  .banner-btn {
    width: 100%;
    color: #fff;
    background: #ed7309;
    margin-top: 15px;
    border: none;
    border-radius: 5px;
  }
`;

const BannerHeader = styled.h1`
  font-size: 16px;
  font-weight: bold;
`;

const BannerText = styled.p`
  font-size: 12px;
  font-weight: bold;
`;

export { BannerWrapper, BannerHeader, BannerText };
