import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

const HomeContainer = styled.div`
  margin: ${isMobileOnly ? "46px" : "110px"} auto;
  max-width: 1040px;
  width: 100%;
  box-sizing: border-box;
  display: flex;
  justify-content: space-between;
  align-items: center;

  @media (max-width: 1024px) {
    flex-direction: column;
  }

  .greeting {
    font-weight: bold;
    margin-bottom: 40px;
  }

  .home-modules-container {
    display: flex;
    flex-direction: column;
    align-items: center;

    @media (max-width: 1024px) {
      order: 2;
    }

    .home-modules {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      grid-gap: 40px ${isMobileOnly ? "32px" : "40px"};

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
