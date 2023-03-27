import styled from "styled-components";

export const StyledMediaError = styled.div`
  position: fixed;
  z-index: 1006;

  left: 50%;
  top: 50%;

  transform: translate(-50%, -50%);

  display: flex;
  justify-content: center;
  align-items: center;

  background: rgba(0, 0, 0, 0.7);

  opacity: 1;
  border-radius: 20px;
  padding: 20px 24px;
`;

export const StyledErrorToolbar = styled.div`
  position: fixed;
  bottom: 24px;
  left: 50%;
  z-index: 1006;

  transform: translateX(-50%);

  display: flex;
  justify-content: center;
  align-items: center;

  padding: 10px 24px;
  border-radius: 18px;

  background: rgba(0, 0, 0, 0.4);

  &:hover {
    background: rgba(0, 0, 0, 0.8);
  }

  svg {
    path {
      fill: white;
    }
  }

  rect: {
    stroke: white;
  }

  .toolbar-item {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 48px;
    width: 48px;
    &:hover {
      cursor: pointer;
    }
  }
`;
