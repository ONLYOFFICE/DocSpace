import styled from "styled-components";

export const ImageViewerToolbarWrapper = styled.div`
  height: 48px;
  padding: 10px 24px;
  border-radius: 18px;

  position: fixed;
  bottom: 24px;
  left: 50%;
  z-index: 307;

  transform: translateX(-50%);

  text-align: center;
  transition: all 0.26s ease-out;

  background: rgba(0, 0, 0, 0.4);
  &:hover {
    background: rgba(0, 0, 0, 0.8);
  }
`;

export const ListTools = styled.ul`
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 0px;
  margin: 0px;
`;
export const ToolbarItem = styled.li<{
  $isSeparator?: boolean;
  $percent?: number;
}>`
  display: flex;
  justify-content: center;
  align-items: center;

  height: 48px;
  width: ${(props) => (props.$isSeparator ? "33px" : "48px")};

  &:hover {
    cursor: ${(props) => (props.$isSeparator ? "default" : "pointer")};
  }

  .zoomPercent {
    font-size: 10px;
    font-weight: 700;
    user-select: none;
  }

  svg {
    width: 16px;
    height: 16px;
    path,
    rect {
      ${(props) => (props.$percent !== 25 ? "fill: #fff;" : "fill: #BEBEBE;")}
    }
  }

  .zoomOut,
  .zoomIn,
  .rotateLeft,
  .rotateRight {
    margin-top: 3px;
  }
`;
