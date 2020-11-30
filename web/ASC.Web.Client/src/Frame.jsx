import React from "react";
//import { Container, Nav, Navbar } from "react-bootstrap";
import { connect } from "react-redux";
//import { Cart } from "react-bootstrap-icons";
import { BrowserRouter as Router, Switch, Route, Link } from "react-router-dom";
// import NavMenu from "@appserver/common/src/components/NavMenu";
import Main from "@appserver/common/src/components/Main";
import Box from "@appserver/components/src/components/box";

const Home = React.lazy(() => import("home/Home"));
// const Search = React.lazy(() => import("search/Search"));
// const Checkout = React.lazy(() => import("checkout/Checkout"));

const HomeRoute = () => (
  <React.Suspense fallback={<div />}>
    <Home />
  </React.Suspense>
);
// const SearchRoute = () => (
//   <React.Suspense fallback={<div />}>
//     <Search />
//   </React.Suspense>
// );
// const CheckoutRoute = () => (
//   <React.Suspense fallback={<div />}>
//     <Checkout />
//   </React.Suspense>
// );

const Frame = ({ items = [], page = "home" }) => (
  <Router>
    <Box>
      {/* <NavMenu /> */}
      <Main>
        <Switch>
          <Route path="/" exact>
            <HomeRoute />
          </Route>
          {/* <Route path="/search">
            <SearchRoute />
          </Route>
          <Route path="/checkout">
            <CheckoutRoute />
          </Route> */}
        </Switch>
      </Main>
    </Box>
  </Router>
);

export default connect((state) => state)(Frame);
