import styled from "styled-components";

const HomeContainer = styled.div`
  margin: 110px auto;
  max-width: 1040px;
  width: 100%;
  box-sizing: border-box;
  display: flex;
  justify-content: space-between;
  align-items: center;

  .home-modules-container {
    display: flex;
    flex-direction: column;
    align-items: center;

    .home-modules {
      display: grid;
      grid-template-columns: repeat(3, 120px);
      max-width: 500px;
      grid-gap: 40px 25px;

      @media (max-width: 1024px) {
        grid-template-columns: repeat(3, 1fr);
      }

      @media (max-width: 500px) {
        grid-gap: 20px 25px;
      }

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

  @media (max-width: 1024px) {
    flex-direction: column;
    margin: 100px auto;

    .home-modules {
      margin: 0 auto;
    }
  }
`;

export default HomeContainer;
