import * as React from "react"
import { graphql } from "gatsby";
import {Trans, useTranslation} from 'gatsby-plugin-react-i18next';
import logo from "./images/santa.svg"
import '../../styles/base.css';
import "./index.css";

const IndexPage = () => {

  const {t, i18n: { language }} = useTranslation("NewYear");

  const origin = "https://www.onlyoffice.com";
  const route = "advent-calendar.aspx"

  const LinkHref =`${origin}/${language === "en" ? route : `${language}/${route}`}`;

  return ( 
  <div>
    <div className="wrapper" style={{display:"flex", margin: "0 auto", backgroundColor: "#266281", minHeight: "60px"}}>
      <img src={logo} width={60}/>
      <div className="content-box" style={{backgroundColor:"#266281", color: "#fff", display: "flex", justifyContent: "center", flexDirection: "column", margin: "0 auto", fontSize: "18px"}}>
      <p>
        {t("Title")}
      </p>
      <p className="text" style={{padding: "0", margin: "0", paddingLeft: "10px", paddingRight: "10px"}}>
        <Trans i18nKey="Text">Get <a target="_blank" style={{color: "#fc9f06"}}
          href={LinkHref}>new gifts and discounts</a>each day - up to 99% off!</Trans>
      </p>

      </div>
      
    </div>
  </div>
  )

}

export default IndexPage


export const query = graphql`
  query ($language: String!) {
    locales: allLocale(filter: {language: {eq: $language}}) {
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