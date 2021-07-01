import styled from "styled-components";

const BannerWrapper = styled.div`
  width: 180px;
  border: 1px solid #d1d1d1;
  border-radius: 5px;
  padding: 15px;
  margin-top: 20px;

  a {
    text-decoration: none;
    color: #000;
  }

  img {
    max-width: 180px;
    height: auto;
    margin-top: 10px;
  }

  .banner-btn {
    width: 100%;
    color: #fff;
    background: #ed7309;
    margin-top: 15px;
    border: none;
    border-radius: 5px;
  }

  .banner-btn:active {
    color: #fff;
    background: #ed7309;
    border: none;
  }
`;

export default BannerWrapper;
