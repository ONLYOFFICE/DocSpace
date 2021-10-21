import styled from "styled-components";

const StyledVersionList = styled.div`

.row_context-menu-wrapper {
    .expandButton {
      ${(props) =>
        props.isRestoreProcess &&
        `
        touch-action: none;
        pointer-events: none;
        `}
      svg {
        path {
          ${(props) =>
            props.isRestoreProcess &&
            `
              fill: #d0d5da;
            `};
        }
      }
    }
  
  }

  .row_content {

    .version_link,
    .version-link-file,
    .version_content-length,
    .version_link-action,
    .row_context-menu-wrapper,
    .version_text {
      ${(props) =>
        props.isRestoreProcess &&
        `
          color: #d0d5da;
          touch-action: none;
          pointer-events: none;
        `}
    }

    .versioned, .not-versioned {
      ${(props) =>
        props.isRestoreProcess &&
        `
        touch-action: none;
        pointer-events: none;
        `}
    }

    .versioned { 
        svg {
            path {
          ${(props) =>
            props.isRestoreProcess &&
            `
              fill: #d0d5da;
            `};
        }
      }
    }

    .not-versioned{
        svg {
            path {

          ${(props) =>
            props.isRestoreProcess &&
            `
              stroke: #d0d5da;
            `};
        }
      }
    }

}
    .icon-link {
      ${(props) =>
        props.isRestoreProcess &&
        `
        touch-action: none;
        pointer-events: none;
        `}
      svg {
        path {
          ${(props) => props.isRestoreProcess && " fill: #d0d5da"}
        }
      }
    }
  }
`;

export default StyledVersionList;
