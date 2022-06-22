import React, {useState, useEffect} from "react";

import { 
    StyledSection, 
    Icon, 
    Name, 
    MainLoader, 
    FilterLoader, 
    SectionContent, 
    SectionHeader, 
    SectionWrapper, 
    StyledIframe, 
    StyledAction } from "./styled-section";

 import cross from './images/cross.react.svg'

export const Section = ({name, origin, lang}) => {

    useEffect(() => {
        setLanguage(lang);
    }, [lang])

    const [language, setLanguage] = useState(lang || "en");
    const url = `${origin}/${language}/${name}`

    const elements = [];

    for (let i = 0; i < 14; i++) {
        elements.push(<SectionContent key={i}>
            <Icon />
            <Name />
            <MainLoader />
        </SectionContent>)
    }

    return <StyledSection>
            <div style={{position: "relative"}}>
                <StyledIframe scrolling="no" className="iframe" src={url} />
                <StyledAction>
                    <img style={{width: "16px"}} src={cross} />
                </StyledAction>
            </div>
            <SectionWrapper>
                <SectionHeader>My documents</SectionHeader>
                <FilterLoader />
                {elements}
            </SectionWrapper>
        </StyledSection>
}