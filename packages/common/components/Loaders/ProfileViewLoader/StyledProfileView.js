import styled from "styled-components";
import { desktop, tablet } from "@docspace/components/utils/device";

const StyledWrapper = styled.div`
  width: 660px;
  display: flex;
  flex-direction: column;
  gap: 40px;

  @media ${tablet} {
    width: 100%;
  }
`;

const MainBlock = styled.div`
  display: flex;
  gap: 40px;
  padding: 24px;
  border-radius: 12px;
  background: linear-gradient(
      270deg,
      rgba(0, 0, 0, 0) 23.13%,
      rgba(0, 0, 0, 0.07) 50.52%,
      rgba(0, 0, 0, 0) 78.12%
    ),
    rgba(0, 0, 0, 0.05);

  .avatar {
    width: 124px;
    height: 124px;
  }

  .combos {
    display: flex;
    gap: 16px;
    flex-direction: column;

    .row {
      display: grid;
      gap: 16px;
      grid-template-columns: 75px 1fr;
    }
  }
`;

const LoginBlock = styled.div`
  display: flex;
  gap: 16px;
  flex-direction: column;

  .title {
    margin-bottom: 4px;
  }

  .actions {
    display: flex;
    gap: 16px;
    align-items: center;
  }
`;

const SocialBlock = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  .row {
    display: flex;
    gap: 20px;

    .button {
      width: 320px;

      @media ${tablet} {
        width: 100%;
      }
    }
  }
`;

const SubBlock = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  .toggle {
    display: flex;
    gap: 12px;
    align-items: center;
  }
`;

const ThemeBlock = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  .checkbox {
    display: flex;
    flex-direction: column;
    gap: 4px;
    margin-bottom: 4px;

    .row {
      display: flex;
      gap: 7px;
      align-items: center;
    }

    .description {
      padding-left: 23px;
    }
  }

  .themes-wrapper {
    display: flex;
    gap: 20px;

    .theme {
      width: 320px;

      @media ${tablet} {
        width: 100%;
      }
    }
  }
`;

const MobileView = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  margin-top: 8px;

  .avatar {
    height: 124px;
    width: 124px;
    margin-bottom: 24px;
    align-self: center;
  }

  .info {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin-bottom: 16px;
  }

  .block {
    display: flex;
    flex-direction: column;
    gap: 4px;
    margin-bottom: 16px;
  }
`;

export {
  StyledWrapper,
  MainBlock,
  LoginBlock,
  SocialBlock,
  SubBlock,
  ThemeBlock,
  MobileView,
};
