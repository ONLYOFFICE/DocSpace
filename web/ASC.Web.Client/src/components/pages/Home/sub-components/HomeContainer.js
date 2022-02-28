import styled, { css } from "styled-components";
import { isMobileOnly, isMobile } from "react-device-detect";

const HomeContainer = styled.div`
  margin: ${isMobileOnly ? "50px" : "42px"} auto 0;
  max-width: 1000px;
  width: 100%;
  box-sizing: border-box;
  display: flex;
  justify-content: ${isMobile ? "center" : "space-between"};
  align-items: center;

  @media (max-width: 1024px) {
    flex-direction: column;
  }

  .greeting {
    font-weight: bold;
    margin-bottom: 28px;
    text-align: center;
    word-break: break-word;
  }

  .home-modules-container {
    display: flex;
    flex-direction: column;
    align-items: center;

    .home-modules {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      grid-gap: 26px ${isMobileOnly ? "31px" : "45px"};

      ${isMobileOnly &&
      css`
        @media (min-width: 500px) {
          display: flex;
          justify-content: center;
          flex-wrap: wrap;
        }
      `}

      .home-module {
        z-index: 42;
      }

      @media (max-width: 400px) {
        display: flex;
        flex-wrap: wrap;
        justify-content: center;
      }
    }

    .home-error-text {
      margin-top: 23px;
      padding: 0 30px;
      @media (min-width: 768px) {
        margin-left: 25%;
        flex: 0 0 50%;
        max-width: 50%;
      }
      @media (min-width: 500px) {
        flex: 0 0 100%;
        max-width: 100%;
      }
    }
  }
`;

export default HomeContainer;
