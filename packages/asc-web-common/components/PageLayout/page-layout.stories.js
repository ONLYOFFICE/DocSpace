import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import styled from "@emotion/styled";
import NavMenu from "../NavMenu";
import Main from "studio/Main";
import PageLayout from ".";
import history from "../../history";
import Headline from "../Headline";
import CatalogItem from "@appserver/components/catalog-item";
import store from "../../store";
import { Provider as MobxProvider } from "mobx-react";
import IconButton from "@appserver/components/icon-button";
import ContextMenuButton from "@appserver/components/context-menu-button";
import MainButton from "@appserver/components/main-button";
import SearchInput from "@appserver/components/search-input";
import Paging from "@appserver/components/paging";
import withReadme from "storybook-readme/with-readme";
import { boolean, withKnobs } from "@storybook/addon-knobs/react";
import Readme from "./README.md";
import { Router } from "react-router-dom";
const { authStore } = store;

const HeaderContent = styled.div`
  display: flex;
  align-items: center;

  & > * {
    margin-right: 8px !important;
  }
`;

const pageItems = [
  {
    key: "1",
    label: "1 of 2",
    onClick: (e) => action("set paging 1 of 2")(e),
  },
  {
    key: "2",
    label: "2 of 2",
    onClick: (e) => action("set paging 2 of 2")(e),
  },
];

const perPageItems = [
  {
    key: "1-1",
    label: "25 per page",
    onClick: (e) => action("set paging 25 action")(e),
  },
  {
    key: "1-2",
    label: "50 per page",
    onClick: (e) => action("set paging 50 action")(e),
  },
];

const articleHeaderContent = <Headline type="menu">Article Header</Headline>;

const catalogHeaderContent = <>Catalog Header</>;

const articleMainButtonContent = (
  <MainButton
    text="Actions"
    clickAction={(e) => action("MainButton Clicked")(e)}
  />
);

const catalogMainButtonContent = (
  <MainButton
    text="Actions"
    clickAction={(e) => action("MainButton Clicked")(e)}
  />
);

const articleBodyContent = <p style={{ padding: 40 }}>Article Content</p>;

const catalogBodyContent = (
  <>
    <CatalogItem
      key={1}
      id={1}
      icon={"static/images/actions.header.touch.react.svg"}
      showText={true}
      text={"Test item"}
      isActive={true}
      onClick={() => {
        console.log("click");
      }}
      isEndOfBlock={true}
      showBadge={false}
      labelBadge={false ? item.newItems : null}
      onClickBadge={() => {
        console.log("badge clicked");
      }}
    ></CatalogItem>
    <CatalogItem
      key={2}
      id={2}
      icon={"static/images/actions.header.touch.react.svg"}
      showText={true}
      text={"Test item"}
      isActive={false}
      onClick={() => {
        console.log("click 2");
      }}
      isEndOfBlock={true}
      showBadge={true}
      labelBadge={false ? 2 : null}
      onClickBadge={() => {
        console.log("badge clicked");
      }}
    ></CatalogItem>
  </>
);

const sectionHeaderContent = (
  <HeaderContent>
    <IconButton
      iconName={"ArrowPathIcon"}
      size="16"
      onClick={(e) => action("ArrowPathIcon Clicked")(e)}
    />
    <Headline type="content">Section Header</Headline>
    <IconButton
      iconName={"static/images/actions.header.touch.react.svg"}
      size="16"
      onClick={(e) => action("PlusIcon Clicked")(e)}
    />
    <ContextMenuButton
      title="Actions"
      getData={() => [
        {
          key: "key",
          label: "label",
          onClick: (e) => action("label Clicked")(e),
        },
      ]}
    />
  </HeaderContent>
);

const sectionFilterContent = (
  <SearchInput
    isNeedFilter={true}
    getFilterData={() => [
      {
        key: "filter-example",
        group: "filter-example",
        label: "example group",
        isHeader: true,
      },
      {
        key: "filter-example-test",
        group: "filter-example",
        label: "Test",
      },
    ]}
    onSearchClick={(result) => {
      console.log(result);
    }}
    onChangeFilter={(result) => {
      console.log(result);
    }}
  />
);

const sectionBodyContent = <p style={{ padding: 40 }}>Section Content</p>;

const sectionPagingContent = (
  <Paging
    previousLabel="Previous"
    nextLabel="Next"
    pageItems={pageItems}
    perPageItems={perPageItems}
    selectedPageItem={pageItems[0]}
    selectedCountItem={perPageItems[0]}
    onSelectPage={(a) => console.log(a)}
    onSelectCount={(a) => console.log(a)}
    previousAction={(e) => action("Prev Clicked")(e)}
    nextAction={(e) => action("Next Clicked")(e)}
    openDirection="top"
  />
);

storiesOf("Components|PageLayout", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <MobxProvider auth={authStore}>
      <Router history={history}>
        <NavMenu
          isBackdropVisible={boolean("isBackdropVisible", false)}
          isNavHoverEnabled={boolean("isNavHoverEnabled", true)}
          isNavOpened={boolean("isNavOpened", false)}
          isAsideVisible={boolean("isAsideVisible", false)}
        />
        <Main>
          <PageLayout withBodyScroll={true}>
            <PageLayout.ArticleHeader>
              {articleHeaderContent}
            </PageLayout.ArticleHeader>

            <PageLayout.ArticleMainButton>
              {articleMainButtonContent}
            </PageLayout.ArticleMainButton>

            <PageLayout.ArticleBody>
              {articleBodyContent}
            </PageLayout.ArticleBody>

            <PageLayout.SectionHeader>
              {sectionHeaderContent}
            </PageLayout.SectionHeader>

            <PageLayout.SectionFilter>
              {sectionFilterContent}
            </PageLayout.SectionFilter>

            <PageLayout.SectionBody>
              {sectionBodyContent}
            </PageLayout.SectionBody>

            <PageLayout.SectionPaging>
              {sectionPagingContent}
            </PageLayout.SectionPaging>
          </PageLayout>
        </Main>
      </Router>
    </MobxProvider>
  ))
  .add("catalog", () => {
    <MobxProvider auth={authStore}>
      <Router history={history}>
        <NavMenu
          isBackdropVisible={boolean("isBackdropVisible", false)}
          isNavHoverEnabled={boolean("isNavHoverEnabled", true)}
          isNavOpened={boolean("isNavOpened", false)}
          isAsideVisible={boolean("isAsideVisible", false)}
        />
        <Main>
          <PageLayout withBodyScroll={true}>
            <PageLayout.CatalogHeader>
              {catalogHeaderContent}
            </PageLayout.CatalogHeader>

            <PageLayout.CatalogMainButton>
              {catalogMainButtonContent}
            </PageLayout.CatalogMainButton>

            <PageLayout.CatalogBody>
              {catalogBodyContent}
            </PageLayout.CatalogBody>

            <PageLayout.SectionHeader>
              {sectionHeaderContent}
            </PageLayout.SectionHeader>

            <PageLayout.SectionFilter>
              {sectionFilterContent}
            </PageLayout.SectionFilter>

            <PageLayout.SectionBody>
              {sectionBodyContent}
            </PageLayout.SectionBody>

            <PageLayout.SectionPaging>
              {sectionPagingContent}
            </PageLayout.SectionPaging>
          </PageLayout>
        </Main>
      </Router>
    </MobxProvider>;
  });
