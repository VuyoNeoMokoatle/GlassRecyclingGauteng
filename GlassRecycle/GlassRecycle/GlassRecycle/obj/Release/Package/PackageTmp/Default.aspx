<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GlassRecycle._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Hero Section -->
    <div class="hero-section">
        <div>
            <h1>Together, We Can Make a Difference</h1>
            <p>Join the movement to recycle glass and protect our planet for future generations.</p>
            <a href="SignIn.aspx" class="btn btn-green mt-3">Get Started</a>
        </div>
    </div>

    <!-- Why Recycle Section -->
    <div class="container text-center mt-5">
        <h2 class="mb-4">Why Recycle Glass?</h2>
        <div class="row">
            <div class="col-md-4">
                <img src="https://cdn-icons-png.flaticon.com/512/2903/2903835.png" alt="Eco" width="90" />
                <h4 class="mt-3">Save Energy</h4>
                <p>Recycling glass saves energy used in creating new materials and reduces carbon emissions.</p>
            </div>
            <div class="col-md-4">
                <img src="https://cdn-icons-png.flaticon.com/512/2903/2903802.png" alt="Reuse" width="90" />
                <h4 class="mt-3">Reuse Efficiently</h4>
                <p>Glass can be recycled endlessly without losing quality — making it a truly sustainable material.</p>
            </div>
            <div class="col-md-4">
                <img src="https://cdn-icons-png.flaticon.com/512/2903/2903824.png" alt="Community" width="90" />
                <h4 class="mt-3">Community Impact</h4>
                <p>Promote clean communities and responsible waste management through recycling programs.</p>
            </div>
        </div>
    </div>

    <!-- Get Involved Section -->
    <div class="container mt-5 text-center">
        <h2>Get Involved</h2>
        <p>Sign up to learn more, participate in recycling events, or contribute to our initiatives.</p>
        <a href="SignUp.aspx" class="btn btn-green mt-3">Join Now</a>
    </div>
</asp:Content>
