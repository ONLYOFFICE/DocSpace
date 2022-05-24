import * as React from "react";
import { graphql } from "gatsby";
import { Trans, useTranslation } from "gatsby-plugin-react-i18next";
import "../../styles/base.css";
import "./index.css";

const IndexPage = () => {
  const {
    t,
    i18n: { language },
  } = useTranslation("Docs_7_1");

  const LinkHrefDocs71 =
    "https://www.onlyoffice.com/blog/2022/05/discover-onlyoffice-docs-v7-1/";

  return (
    <>
      <div className={"advent-announce advent-mobile-hide " + language}>
        <a className="docs-7-1" target="_blank" href={LinkHrefDocs71}>
          <div className="advent-announce-text">
            <div>
              <Trans i18nKey="BannerTextDesktop">
                <b>
                  Discover <span>ONLYOFFICE Docs v7.1</span>:
                </b>{" "}
                ARM compatibility, upgraded PDF/XPS/DjVu viewer, PDF to DOCX
                conversion, Print preview for sheets and more
              </Trans>
            </div>
          </div>
        </a>
      </div>
      <div className="advent-announce advent-desktop-hide">
        <a className="docs-7-1" target="_blank" href={LinkHrefDocs71}>
          <div className="advent-announce-text">
            <Trans i18nKey="BannerTextMob">
              <b>
                Discover ONLYOFFICE <span>Docs v7.1</span>
              </b>
            </Trans>
          </div>
        </a>
      </div>
    </>
  );
};

export default IndexPage;

export const query = graphql`
  query($language: String!) {
    locales: allLocale(filter: { language: { eq: $language } }) {
      edges {
        node {
          ns
          data
          language
        }
      }
    }
  }
`;
