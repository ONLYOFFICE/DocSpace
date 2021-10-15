import styled, { css } from 'styled-components';
import { isMobile, isMobileOnly } from 'react-device-detect';
import { tablet, desktop, mobile } from '@appserver/components/utils/device';

const StyledContainer = styled.div`
  .header-container {
    position: relative;
    height: ${isMobile ? (isMobileOnly ? '53px !important' : '61px !important') : '53px'};
    align-items: center;
    max-width: ${(props) => props.width}px;
    @media ${tablet} {
      height: 63px;
    }

    @media ${mobile} {
      height: 53px;
    }
    ${(props) =>
      props.title &&
      css`
        display: grid;
        grid-template-columns: ${(props) =>
          props.isRootFolder
            ? 'auto auto 1fr'
            : props.canCreate
            ? 'auto auto auto auto 1fr'
            : 'auto auto auto 1fr'};

        @media ${tablet} {
          grid-template-columns: ${(props) =>
            props.isRootFolder
              ? '1fr auto'
              : props.canCreate
              ? 'auto 1fr auto auto'
              : 'auto 1fr auto'};
        }
      `}

    .arrow-button {
      margin-right: 15px;
      min-width: 17px;
      padding: 16px 0 12px;
      align-items: center;

      @media ${tablet} {
        padding: 20px 0 16px 8px;
        margin-left: -8px;
        margin-right: 16px;
      }
    }

    .add-button {
      margin-bottom: -1px;
      margin-left: 16px;

      @media ${tablet} {
        margin-left: auto;

        & > div:first-child {
          padding: 8px 8px 8px 8px;
          margin-right: -8px;
        }
      }
    }

    .option-button {
      margin-bottom: -1px;

      @media (min-width: 1024px) {
        margin-left: 8px;
      }

      @media ${tablet} {
        & > div:first-child {
          padding: 8px 8px 8px 8px;
          margin-right: -8px;
        }
      }
    }
  }
`;

export default StyledContainer;
