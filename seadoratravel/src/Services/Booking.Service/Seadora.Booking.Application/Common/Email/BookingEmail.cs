using System;
using System.Collections.Generic;
using System.Text;
using Seadora.Booking.Domain.Entities;

namespace Seadora.Booking.Application.Common.Email;

public static class BookingEmail
{
    // High-Luminance 24K Gold & Radiant Luxury Palette (Armored against Gmail iOS Dark Mode Dimming)
    private const string GoldRadiant = "#FFD768";
    private const string GoldBright = "#FFE58F";
    private const string GoldAmber = "#FFAE19";
    private const string NavyDarkest = "#050B12";
    private const string NavyMain = "#07172B";
    private const string NavyCard = "#0B1E34";
    private const string NavyTicket = "#0F2844";
    private const string TextRadiantWhite = "#FFFFFF";
    private const string TextIceWhite = "#E6F0FA";
    private const string TextMutedAzure = "#ADC5DB";
    private const string GreenEmerald = "#10B981";
    private const string GreenBright = "#34D399";
    private const string LogoUrl = "https://seadoratravel.com/logo-emblem.png";
    private const string WhatsAppUrl = ContactChannels.WhatsAppUrl;
    private const string WebsiteUrl = ContactChannels.WebsiteUrl;

    private class EmailStrings
    {
        public string HeaderSubtitle { get; set; } = "";
        public string PreheaderReceived { get; set; } = "";
        public string StatusReceived { get; set; } = "";
        public string TitleReceived { get; set; } = "";
        public string Greeting { get; set; } = "";
        public string ReceivedIntro { get; set; } = "";
        public string PreheaderConfirmed { get; set; } = "";
        public string StatusConfirmed { get; set; } = "";
        public string TitleConfirmed { get; set; } = "";
        public string ConfirmedIntro { get; set; } = "";
        public string VoucherRef { get; set; } = "";
        public string OfficialVoucher { get; set; } = "";
        public string TourDate { get; set; } = "";
        public string PickupWindow { get; set; } = "";
        public string Guests { get; set; } = "";
        public string TotalAmount { get; set; } = "";
        public string PickupLocation { get; set; } = "";
        public string WhatNextTitle { get; set; } = "";
        public string WhatNextDesc { get; set; } = "";
        public string RemindersTitle { get; set; } = "";
        public string Reminder1 { get; set; } = "";
        public string Reminder2 { get; set; } = "";
        public string Reminder3 { get; set; } = "";
        public string FlexibilityTitle { get; set; } = "";
        public string FlexibilityDesc { get; set; } = "";
        public string ConciergeTitle { get; set; } = "";
        public string ConciergeDesc { get; set; } = "";
        public string WhatsAppCta { get; set; } = "";
        public string FooterMarina { get; set; } = "";
        public string FooterRights { get; set; } = "";
        public string DefaultExperience { get; set; } = "";
        public string DateTbd { get; set; } = "";
        public string PickupTbd { get; set; } = "";
        public string SelectedAddonsLabel { get; set; } = "";
        public string GuestsBreakdownLabel { get; set; } = "";
    }

    private static readonly Dictionary<string, EmailStrings> Localizations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = new EmailStrings
        {
            HeaderSubtitle = "Luxury Red Sea Concierge & Private Journeys",
            PreheaderReceived = "Your VIP reservation request has been received and is being prepared.",
            StatusReceived = "Reservation Request Received",
            TitleReceived = "Booking Received — Seadora Travel",
            Greeting = "Dear",
            ReceivedIntro = "Thank you for selecting <strong>Seadora Luxury Travel</strong>. We have received your reservation request and our VIP operations team in Hurghada is reviewing your schedule to ensure every detail meets our five-star standards.",
            PreheaderConfirmed = "Your VIP Booking is CONFIRMED! View your official travel voucher inside.",
            StatusConfirmed = "Officially Confirmed",
            TitleConfirmed = "Booking Confirmed — Seadora Travel",
            ConfirmedIntro = "We are delighted to confirm your luxury experience with <strong>Seadora Travel</strong>. Your private vehicle, licensed guide, and bespoke arrangements are fully secured. Please keep this voucher accessible on your phone.",
            VoucherRef = "VOUCHER REFERENCE",
            OfficialVoucher = "OFFICIAL TRAVEL VOUCHER",
            TourDate = "Date of Experience",
            PickupWindow = "Pickup Window",
            Guests = "Party Size",
            TotalAmount = "Total Amount",
            PickupLocation = "Meeting / Pickup Location",
            WhatNextTitle = "What Happens Next?",
            WhatNextDesc = "Our concierge will finalize details with your private chauffeur and crew. You will receive real-time updates and your driver's contact info via WhatsApp prior to departure.",
            RemindersTitle = "Important Reminders for Tour Day",
            Reminder1 = "Please carry a physical passport/ID or a clear photo on your mobile phone.",
            Reminder2 = "Please be present in your hotel lobby 10 minutes prior to the pickup window.",
            Reminder3 = "Comfortable resort wear, sunscreen, and sunglasses are highly recommended.",
            FlexibilityTitle = "Guaranteed Flexibility",
            FlexibilityDesc = "Free cancellation and 100% full refund available up to 48 hours prior to departure. Need date adjustments? Message our concierge anytime.",
            ConciergeTitle = "Dedicated 24/7 VIP Concierge",
            ConciergeDesc = "Our local Hurghada travel specialists are on standby to accommodate any bespoke requests or dietary preferences.",
            WhatsAppCta = "💬 Chat with Concierge on WhatsApp",
            FooterMarina = "Hurghada Marina, Red Sea Governorate, Egypt",
            FooterRights = "All rights reserved. • High-End Hospitality by SEADORA",
            DefaultExperience = "Private VIP Experience",
            DateTbd = "Date on Request",
            PickupTbd = "To be confirmed by Concierge",
            SelectedAddonsLabel = "Selected Upgrades & Add-ons",
            GuestsBreakdownLabel = "Registered VIP Guests"
        },
        ["de"] = new EmailStrings
        {
            HeaderSubtitle = "Luxus Rotes Meer Concierge & Private Reisen",
            PreheaderReceived = "Ihre VIP-Reservierungsanfrage ist eingegangen und wird vorbereitet.",
            StatusReceived = "Reservierungsanfrage Eingegangen",
            TitleReceived = "Buchung Eingegangen — Seadora Travel",
            Greeting = "Sehr geehrte(r)",
            ReceivedIntro = "Vielen Dank, dass Sie sich für <strong>Seadora Luxury Travel</strong> entschieden haben. Wir haben Ihre Buchungsanfrage erhalten. Unser VIP-Team in Hurghada prüft derzeit Ihren Zeitplan, um erstklassige Exzellenz zu gewährleisten.",
            PreheaderConfirmed = "Ihre VIP-Buchung ist BESTÄTIGT! Ihr offizieller Reisegutschein im Anhang.",
            StatusConfirmed = "Offiziell Bestätigt",
            TitleConfirmed = "Buchung Bestätigt — Seadora Travel",
            ConfirmedIntro = "Wir freuen uns sehr, Ihr Luxuserlebnis mit <strong>Seadora Travel</strong> zu bestätigen. Ihr Privatfahrzeug, Reiseleiter und alle VIP-Arrangements sind gesichert. Bitte halten Sie diesen Voucher bereit.",
            VoucherRef = "VOUCHER-REFERENZ",
            OfficialVoucher = "OFFIZIELLER REISEGUTSCHEIN",
            TourDate = "Datum des Erlebnisses",
            PickupWindow = "Abholzeitfenster",
            Guests = "Teilnehmerzahl",
            TotalAmount = "Gesamtbetrag",
            PickupLocation = "Treffpunkt / Abholort",
            WhatNextTitle = "Was passiert als Nächstes?",
            WhatNextDesc = "Unser Concierge finalisiert alle Details mit Ihrem Chauffeur. Sie erhalten rechtzeitig vor der Abfahrt alle Fahrerdaten und Live-Updates via WhatsApp.",
            RemindersTitle = "Wichtige Hinweise für den Tourtag",
            Reminder1 = "Bitte führen Sie Ihren Reisepass/Ausweis oder ein klares Foto auf dem Smartphone mit.",
            Reminder2 = "Bitte finden Sie sich 10 Minuten vor der geplanten Zeit in der Hotellobby ein.",
            Reminder3 = "Bequeme Kleidung, Sonnenschutz und Sonnenbrille werden wärmstens empfohlen.",
            FlexibilityTitle = "Garantierte Flexibilität",
            FlexibilityDesc = "Kostenlose Stornierung und 100% Rückerstattung bis zu 48 Stunden vor Beginn. Datumsänderungen sind jederzeit über unseren Concierge möglich.",
            ConciergeTitle = "Dedizierter 24/7 VIP-Concierge",
            ConciergeDesc = "Unsere Reisespezialisten in Hurghada stehen Ihnen jederzeit für Sonderwünsche zur Verfügung.",
            WhatsAppCta = "💬 Mit Concierge auf WhatsApp chatten",
            FooterMarina = "Hurghada Marina, Rotes Meer, Ägypten",
            FooterRights = "Alle Rechte vorbehalten. • Exzellenter Service von SEADORA",
            DefaultExperience = "Privates VIP-Erlebnis",
            DateTbd = "Datum auf Anfrage",
            PickupTbd = "Wird vom Concierge bestätigt",
            SelectedAddonsLabel = "Gewählte Upgrades & Extras",
            GuestsBreakdownLabel = "Registrierte VIP-Gäste"
        },
        ["it"] = new EmailStrings
        {
            HeaderSubtitle = "Concierge di Lusso del Mar Rosso & Viaggi Privati",
            PreheaderReceived = "La tua richiesta di prenotazione VIP è stata ricevuta.",
            StatusReceived = "Richiesta Ricevuta",
            TitleReceived = "Prenotazione Ricevuta — Seadora Travel",
            Greeting = "Gentile",
            ReceivedIntro = "Grazie per aver scelto <strong>Seadora Luxury Travel</strong>. Abbiamo ricevuto la tua richiesta di prenotazione e il nostro team VIP a Hurghada sta organizzando ogni dettaglio secondo i più alti standard.",
            PreheaderConfirmed = "La tua prenotazione VIP è CONFERMATA! Visualizza il tuo voucher ufficiale.",
            StatusConfirmed = "Ufficialmente Confermata",
            TitleConfirmed = "Prenotazione Confermata — Seadora Travel",
            ConfirmedIntro = "Siamo lieti di confermare la tua esperienza di lusso con <strong>Seadora Travel</strong>. Il tuo veicolo privato, la guida e tutti i servizi sono confermati.",
            VoucherRef = "RIFERIMENTO VOUCHER",
            OfficialVoucher = "VOUCHER DI VIAGGIO UFFICIALE",
            TourDate = "Data dell'Esperienza",
            PickupWindow = "Orario di Prelievo",
            Guests = "Numero Ospiti",
            TotalAmount = "Importo Totale",
            PickupLocation = "Luogo di Prelievo / Hotel",
            WhatNextTitle = "Cosa Succede Ora?",
            WhatNextDesc = "Il nostro concierge coordinerà i dettagli finali con l'autista privato. Riceverai aggiornamenti e contatti diretti via WhatsApp prima della partenza.",
            RemindersTitle = "Promemoria Importanti per la Giornata",
            Reminder1 = "Porta con te il passaporto/documento o una foto nitida sul cellulare.",
            Reminder2 = "Presentati nella hall dell'hotel 10 minuti prima dell'orario concordato.",
            Reminder3 = "Si consigliano abbigliamento comodo, protezione solare e occhiali da sole.",
            FlexibilityTitle = "Flessibilità Garantita",
            FlexibilityDesc = "Cancellazione gratuita con rimborso al 100% fino a 48 ore prima della partenza.",
            ConciergeTitle = "Concierge VIP Dedicato 24/7",
            ConciergeDesc = "I nostri specialisti di Hurghada sono sempre a tua completa disposizione.",
            WhatsAppCta = "💬 Chatta con il Concierge su WhatsApp",
            FooterMarina = "Marina di Hurghada, Mar Rosso, Egitto",
            FooterRights = "Tutti i diritti riservati. • Eccellenza SEADORA",
            DefaultExperience = "Esperienza Privata VIP",
            DateTbd = "Data su richiesta",
            PickupTbd = "Da confermare dal Concierge",
            SelectedAddonsLabel = "Servizi Aggiuntivi Selezionati",
            GuestsBreakdownLabel = "Ospiti VIP Registrati"
        },
        ["fr"] = new EmailStrings
        {
            HeaderSubtitle = "Conciergerie de Luxe Mer Rouge & Voyages Privés",
            PreheaderReceived = "Votre demande de réservation VIP a bien été reçue.",
            StatusReceived = "Demande Reçue",
            TitleReceived = "Réservation Reçue — Seadora Travel",
            Greeting = "Cher(ère)",
            ReceivedIntro = "Merci d'avoir choisi <strong>Seadora Luxury Travel</strong>. Nous avons bien reçu votre demande de réservation. Notre équipe VIP à Hurghada prépare minutieusement votre voyage d'exception.",
            PreheaderConfirmed = "Votre réservation VIP est CONFIRMÉE ! Consultez votre bon officiel.",
            StatusConfirmed = "Officiellement Confirmée",
            TitleConfirmed = "Réservation Confirmée — Seadora Travel",
            ConfirmedIntro = "Nous sommes ravis de confirmer votre expérience de prestige avec <strong>Seadora Travel</strong>. Votre véhicule privé et votre guide dédié sont réservés.",
            VoucherRef = "RÉFÉRENCE DU BON",
            OfficialVoucher = "BON DE VOYAGE OFFICIEL",
            TourDate = "Date de l'Expérience",
            PickupWindow = "Créneau de Prise en Charge",
            Guests = "Nombre de Voyageurs",
            TotalAmount = "Montant Total",
            PickupLocation = "Lieu de Prise en Charge / Hôtel",
            WhatNextTitle = "Prochaines Étapes",
            WhatNextDesc = "Notre conciergerie finalise les détails avec votre chauffeur privé. Vous recevrez toutes les coordonnées directes par WhatsApp avant le départ.",
            RemindersTitle = "Conseils Importants pour le Jour J",
            Reminder1 = "Veuillez vous munir de votre passeport ou d'une photo claire sur smartphone.",
            Reminder2 = "Rendez-vous dans le hall de votre hôtel 10 minutes avant l'heure prévue.",
            Reminder3 = "Tenue décontractée, crème solaire et lunettes de soleil sont vivement conseillées.",
            FlexibilityTitle = "Flexibilité Garantie",
            FlexibilityDesc = "Annulation gratuite avec remboursement à 100% jusqu'à 48h avant le départ.",
            ConciergeTitle = "Conciergerie VIP Dédiée 24/7",
            ConciergeDesc = "Nos spécialistes à Hurghada restent à votre entière disposition.",
            WhatsAppCta = "💬 Discuter avec la Conciergerie sur WhatsApp",
            FooterMarina = "Marina d'Hurghada, Mer Rouge, Égypte",
            FooterRights = "Tous droits réservés. • Prestations d'Excellence par SEADORA",
            DefaultExperience = "Expérience VIP Privée",
            DateTbd = "Date sur demande",
            PickupTbd = "À confirmer par la conciergerie",
            SelectedAddonsLabel = "Options & Suppléments",
            GuestsBreakdownLabel = "Voyageurs VIP Enregistrés"
        },
        ["ru"] = new EmailStrings
        {
            HeaderSubtitle = "VIP Консьерж-сервис на Красном Море и Индивидуальные Туры",
            PreheaderReceived = "Ваш VIP-запрос на бронирование получен и обрабатывается.",
            StatusReceived = "Запрос на Бронирование Получен",
            TitleReceived = "Бронирование Получено — Seadora Travel",
            Greeting = "Уважаемый(ая)",
            ReceivedIntro = "Благодарим вас за выбор <strong>Seadora Luxury Travel</strong>. Мы получили ваш запрос на бронирование. Наша VIP-команда в Хургаде уже готовит поездку по высочайшим стандартам пятизвездочного сервиса.",
            PreheaderConfirmed = "Ваше VIP-бронирование ПОДТВЕРЖДЕНО! Ваш официальный ваучер внутри.",
            StatusConfirmed = "Официально Подтверждено",
            TitleConfirmed = "Бронирование Подтверждено — Seadora Travel",
            ConfirmedIntro = "Мы рады подтвердить ваше эксклюзивное путешествие с <strong>Seadora Travel</strong>. Индивидуальный комфортабельный трансфер и персональный гид закреплены за вами.",
            VoucherRef = "НОМЕР ВАУЧЕРА",
            OfficialVoucher = "ОФИЦИАЛЬНЫЙ ТУРИСТИЧЕСКИЙ ВАУЧЕР",
            TourDate = "Дата Поездки",
            PickupWindow = "Время Трансфера из Отеля",
            Guests = "Количество Гостей",
            TotalAmount = "Итоговая Сумма",
            PickupLocation = "Место Сбора / Отель",
            WhatNextTitle = "Что Происходит Дальше?",
            WhatNextDesc = "Консьерж согласует все детали с водителем и экипажем. Перед выездом вы получите контакты водителя и уведомление в WhatsApp.",
            RemindersTitle = "Важные Рекомендации на День Экскурсии",
            Reminder1 = "Возьмите с собой оригинал паспорта или четкое фото на смартфоне.",
            Reminder2 = "Пожалуйста, ожидайте в лобби отеля за 10 минут до назначенного времени трансфера.",
            Reminder3 = "Рекомендуем удобную обувь, солнцезащитные очки и крем от солнца.",
            FlexibilityTitle = "Гарантия Гибкости",
            FlexibilityDesc = "Бесплатная отмена и 100% возврат средств за 48 часов до выезда. Изменение даты доступно в любое время через консьержа.",
            ConciergeTitle = "Персональный VIP-Консьерж 24/7",
            ConciergeDesc = "Наши специалисты в Хургаде круглосуточно на связи для любых пожеланий.",
            WhatsAppCta = "💬 Написать Консьержу в WhatsApp",
            FooterMarina = "Марина Хургада, провинция Красное Море, Египет",
            FooterRights = "Все права защищены. • Премиальный сервис SEADORA",
            DefaultExperience = "Индивидуальный VIP Тур",
            DateTbd = "Дата по согласованию",
            PickupTbd = "Будет уточнено консьержем",
            SelectedAddonsLabel = "Выбранные Дополнительные Опции",
            GuestsBreakdownLabel = "Список Зарегистрированных Гостей"
        }
    };

    private static EmailStrings GetStrings(string? lang)
    {
        if (string.IsNullOrWhiteSpace(lang)) return Localizations["en"];
        var key = lang.Trim().ToLowerInvariant();
        return Localizations.TryGetValue(key, out var s) ? s : Localizations["en"];
    }

    private static string GetEmailHeader(EmailStrings s, string preheader, string statusPill, string title, bool isConfirmed = false)
    {
        var badgeBg = isConfirmed 
            ? "background-color: #064E3B; background-image: linear-gradient(135deg, #059669 0%, #047857 100%); border: 1.5px solid #34D399; color: #FFFFFF;"
            : "background-color: #3D2800; background-image: linear-gradient(135deg, #B45309 0%, #78350F 100%); border: 1.5px solid #FFD768; color: #FFF4CC;";

        return $@"
        <!DOCTYPE html>
        <html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <meta name='color-scheme' content='light dark'>
            <meta name='supported-color-schemes' content='light dark'>
            <title>{title}</title>
            <style>
                :root {{
                    color-scheme: light dark;
                    supported-color-schemes: light dark;
                }}
                @import url('https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,600;0,700;0,800;1,400&family=Jost:wght@400;500;600;700&display=swap');
                
                body {{
                    margin: 0;
                    padding: 0;
                    background-color: {NavyDarkest};
                    font-family: 'Jost', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                    -webkit-font-smoothing: antialiased;
                    color: {TextIceWhite};
                }}
                table {{ border-collapse: separate; }}
                a {{ text-decoration: none; }}
                
                /* Dark Mode & Client Armor */
                @media (prefers-color-scheme: dark) {{
                    body, .email-bg {{ background-color: {NavyDarkest} !important; }}
                    .radiant-gold {{ color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; }}
                    .radiant-white {{ color: {TextRadiantWhite} !important; -webkit-text-fill-color: {TextRadiantWhite} !important; }}
                    .radiant-body {{ color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important; }}
                    .radiant-muted {{ color: {TextMutedAzure} !important; -webkit-text-fill-color: {TextMutedAzure} !important; }}
                }}
                
                /* Outlook OWA */
                [data-ogsc] .radiant-gold {{ color: {GoldRadiant} !important; }}
                [data-ogsc] .radiant-white {{ color: {TextRadiantWhite} !important; }}
                [data-ogsc] .radiant-body {{ color: {TextIceWhite} !important; }}
                [data-ogsc] .radiant-muted {{ color: {TextMutedAzure} !important; }}
                [data-ogsb] .email-bg {{ background-color: {NavyDarkest} !important; }}

                /* iPhone Gmail App Armor (u + .body) */
                u + .body .email-bg {{ background-color: {NavyDarkest} !important; }}
                u + .body .radiant-gold {{ color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; }}
                u + .body .radiant-white {{ color: {TextRadiantWhite} !important; -webkit-text-fill-color: {TextRadiantWhite} !important; }}
                u + .body .radiant-body {{ color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important; }}
                u + .body .radiant-muted {{ color: {TextMutedAzure} !important; -webkit-text-fill-color: {TextMutedAzure} !important; }}

                @media only screen and (max-width: 620px) {{
                    .email-container {{ width: 100% !important; border-radius: 0 !important; }}
                    .mobile-p-20 {{ padding: 24px 18px !important; }}
                    .mobile-col {{ display: block !important; width: 100% !important; padding-bottom: 14px !important; }}
                }}
            </style>
        </head>
        <body class='body' style='margin: 0; padding: 0; background-color: {NavyDarkest}; -webkit-text-size-adjust: 100%;'>
            <!-- Hidden Preheader for Mail Inboxes -->
            <div style='display: none; font-size: 1px; color: #fff; line-height: 1px; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;'>
                {preheader}
            </div>

            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' class='email-bg' style='background-color: {NavyDarkest}; background-image: linear-gradient({NavyDarkest}, {NavyDarkest}); width: 100%; padding: 32px 0;'>
                <tr>
                    <td align='center'>
                        <!-- Main Luxury Card Container with Radiant Gold Border -->
                        <table role='presentation' class='email-container' width='600' cellspacing='0' cellpadding='0' border='0' style='width: 600px; max-width: 600px; background-color: {NavyMain}; background-image: linear-gradient(180deg, #0A1E36 0%, #061324 100%); border-radius: 20px; overflow: hidden; border: 1.5px solid {GoldRadiant}; box-shadow: 0 16px 48px rgba(0,0,0,0.5);'>
                            
                            <!-- Luxury Header with Glowing Gold Typography -->
                            <tr>
                                <td style='background-color: {NavyDarkest}; background-image: linear-gradient(135deg, #06152B 0%, #0D2342 100%); padding: 38px 28px; text-align: center; border-bottom: 2.5px solid {GoldRadiant};'>
                                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                        <tr>
                                            <td align='center'>
                                                <!-- Emblem -->
                                                <img src='{LogoUrl}' alt='Seadora Emblem' width='58' height='58' style='display: block; margin: 0 auto 16px auto; width: 58px; height: 58px; filter: drop-shadow(0 4px 16px rgba(255,215,104,0.6));' />

                                                <!-- 24K Radiant Gold Brand Title (Never Dims) -->
                                                <h1 class='radiant-gold' style='margin: 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 27px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; letter-spacing: 4px; text-transform: uppercase; text-shadow: 0 0 20px rgba(255,215,104,0.6);'>
                                                    <span style='color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important;'>SEADORA TRAVEL</span>
                                                </h1>
                                                <div class='radiant-gold' style='margin: 6px auto 8px auto; color: {GoldRadiant} !important; font-size: 10px; letter-spacing: 6px;'>
                                                    ✦ • ✦
                                                </div>
                                                <p class='radiant-gold' style='margin: 0; font-size: 11px; font-weight: 700; color: {GoldBright} !important; -webkit-text-fill-color: {GoldBright} !important; letter-spacing: 2px; text-transform: uppercase;'>
                                                    <span style='color: {GoldBright} !important; -webkit-text-fill-color: {GoldBright} !important;'>{s.HeaderSubtitle}</span>
                                                </p>

                                                <!-- Status Badge Pill -->
                                                <div style='margin-top: 20px;'>
                                                    <span style='display: inline-block; padding: 7px 24px; {badgeBg} border-radius: 30px; font-size: 11px; font-weight: 800; letter-spacing: 1.5px; text-transform: uppercase; box-shadow: 0 4px 14px rgba(0,0,0,0.4);'>
                                                        {statusPill}
                                                    </span>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>";
    }

    private static string GetEmailFooter(EmailStrings s)
    {
        return $@"
                            <!-- Concierge Direct Action Bar -->
                            <tr>
                                <td style='background-color: {NavyCard}; background-image: linear-gradient(180deg, #0E2540 0%, #0A1C30 100%); padding: 30px 34px; border-top: 1.5px solid {GoldRadiant}; text-align: center;'>
                                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                        <tr>
                                            <td align='center'>
                                                <p class='radiant-gold' style='margin: 0 0 8px 0; font-size: 13px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1.5px;'>
                                                    <span style='color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important;'>{s.ConciergeTitle}</span>
                                                </p>
                                                <p class='radiant-body' style='margin: 0 0 18px 0; font-size: 13px; color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important; line-height: 1.5;'>
                                                    <span style='color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important;'>{s.ConciergeDesc}</span>
                                                </p>
                                                <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center'>
                                                    <tr>
                                                        <td style='border-radius: 12px; background-color: #25D366; background-image: linear-gradient(135deg, #25D366 0%, #128C7E 100%); text-align: center; box-shadow: 0 6px 20px rgba(37,211,102,0.35);'>
                                                            <a href='{WhatsAppUrl}' target='_blank' style='display: inline-block; padding: 14px 30px; font-size: 13px; font-weight: 800; color: #ffffff !important; -webkit-text-fill-color: #ffffff !important; letter-spacing: 0.5px;'>
                                                                <span style='color: #ffffff !important; -webkit-text-fill-color: #ffffff !important;'>{s.WhatsAppCta}</span>
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <!-- Elevated Brand Dark Footer -->
                            <tr>
                                <td style='background-color: {NavyDarkest}; background-image: linear-gradient(180deg, #061324 0%, #03080E 100%); padding: 32px 24px; text-align: center; color: {TextMutedAzure}; font-size: 12px; line-height: 1.6; border-top: 1px solid rgba(255,215,104,0.3);'>
                                    <p class='radiant-white' style='margin: 0 0 6px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 15px; font-weight: 700; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; letter-spacing: 2px;'>
                                        <span style='color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important;'>SEADORA LUXURY TRAVEL</span>
                                    </p>
                                    <p class='radiant-muted' style='margin: 0 0 10px 0; font-size: 11px; color: {TextMutedAzure} !important; -webkit-text-fill-color: {TextMutedAzure} !important;'>
                                        <span style='color: {TextMutedAzure} !important;'>{s.FooterMarina} • Tel: {ContactChannels.WhatsAppNumber}</span>
                                    </p>
                                    <p style='margin: 0 0 16px 0; color: {TextMutedAzure}; font-size: 11px;'>
                                        Inquiries: <a href='mailto:{ContactChannels.InfoEmail}' style='color: {GoldRadiant}; font-weight: 700;'><span style='color: {GoldRadiant};'>{ContactChannels.InfoEmail}</span></a> • Website: <a href='{WebsiteUrl}' style='color: {GoldRadiant}; font-weight: 700;'><span style='color: {GoldRadiant};'>{WebsiteUrl}</span></a>
                                    </p>
                                    <div style='border-top: 1px solid rgba(255,255,255,0.1); padding-top: 16px; font-size: 11px; color: rgba(255,255,255,0.4);'>
                                        &copy; {DateTime.UtcNow.Year} Seadora Travel. {s.FooterRights}
                                    </div>
                                </td>
                            </tr>

                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
    }

    public static string BuildReceiptHtml(Domain.Entities.Booking booking)
    {
        var s = GetStrings(booking.Language);
        var refCode = booking.Id.ToString().Substring(0, 8).ToUpper();
        var tourDateFormatted = booking.TourDate.HasValue ? booking.TourDate.Value.ToString("dddd, MMMM dd, yyyy") : s.DateTbd;
        var pickupInfo = string.IsNullOrWhiteSpace(booking.PickupTime) ? s.PickupTbd : booking.PickupTime;
        var hotelInfo = string.IsNullOrWhiteSpace(booking.HotelName) ? "Hotel Pickup" : $"{booking.HotelName} {(string.IsNullOrWhiteSpace(booking.RoomNumber) ? "" : $"(Room {booking.RoomNumber})")}";

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            s: s,
            preheader: s.PreheaderReceived,
            statusPill: s.StatusReceived,
            title: s.TitleReceived,
            isConfirmed: false
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20' style='padding: 38px 36px 28px 36px;'>
                <p class='radiant-gold' style='margin: 0 0 6px 0; font-size: 12px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1.5px;'>
                    <span style='color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important;'>★ VIP Hospitality</span>
                </p>
                <h2 class='radiant-white' style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; line-height: 1.3;'>
                    <span style='color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important;'>{s.Greeting} {booking.CustomerName},</span>
                </h2>
                <p class='radiant-body' style='margin: 0 0 28px 0; font-size: 14.5px; color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important; line-height: 1.75;'>
                    <span style='color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important;'>{s.ReceivedIntro}</span>
                </p>

                <!-- Boarding Pass Style Ticket Card (Armored Gradient) -->
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: {NavyTicket}; background-image: linear-gradient(180deg, #112C4C 0%, #0A1C30 100%); border: 1.5px solid {GoldRadiant}; border-radius: 16px; overflow: hidden; margin-bottom: 28px; box-shadow: 0 8px 24px rgba(0,0,0,0.4);'>
                    <tr>
                        <td style='background-color: #06152B; background-image: linear-gradient(145deg, #0D2644 0%, #06152B 100%); padding: 18px 22px; border-bottom: 1.5px solid {GoldRadiant};'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td>
                                        <span class='radiant-gold' style='font-size: 10px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1.5px;'>
                                            <span style='color: {GoldRadiant} !important;'>{s.VoucherRef}</span>
                                        </span>
                                        <div class='radiant-white' style='font-family: ""Playfair Display"", serif; font-size: 21px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; letter-spacing: 2px;'>
                                            <span style='color: #FFFFFF !important;'>#{refCode}</span>
                                        </div>
                                    </td>
                                    <td align='right'>
                                        <span style='display: inline-block; padding: 6px 16px; background-color: rgba(255,215,104,0.15); border: 1px solid {GoldRadiant}; border-radius: 20px; font-size: 11px; font-weight: 700; color: {GoldBright};'>
                                            <span style='color: {GoldBright};'>{(booking.TripType ?? s.DefaultExperience)}</span>
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 24px 22px;'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td class='mobile-col' width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>{s.TourDate}</span></div>
                                        <div class='radiant-white' style='font-size: 15px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; margin-top: 4px;'><span style='color: #FFFFFF;'>{tourDateFormatted}</span></div>
                                    </td>
                                    <td class='mobile-col' width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>{s.PickupWindow}</span></div>
                                        <div class='radiant-white' style='font-size: 15px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; margin-top: 4px;'><span style='color: #FFFFFF;'>{pickupInfo}</span></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='mobile-col' width='50%' valign='top'>
                                        <div class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>{s.Guests}</span></div>
                                        <div class='radiant-white' style='font-size: 15px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; margin-top: 4px;'><span style='color: #FFFFFF;'>{booking.Guests}</span></div>
                                    </td>
                                    <td class='mobile-col' width='50%' valign='top'>
                                        <div class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>{s.TotalAmount}</span></div>
                                        <div class='radiant-gold' style='font-size: 20px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; margin-top: 4px;'><span style='color: {GoldRadiant};'>${booking.TotalPrice:N2}</span></div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #081626; padding: 14px 22px; border-top: 1px dashed rgba(255,215,104,0.3); font-size: 13px; color: {TextIceWhite};'>
                            <strong style='color: {GoldRadiant};'>📍 {s.PickupLocation}:</strong> <span style='color: {TextIceWhite};'>{hotelInfo}</span>
                        </td>
                    </tr>
                </table>");

        // Optional Add-ons section
        if (booking.SelectedAddons != null && booking.SelectedAddons.Count > 0)
        {
            sb.Append($@"
                <div style='margin-bottom: 26px; padding: 18px 22px; background-color: {NavyTicket}; background-image: linear-gradient(#112C4C, #112C4C); border-radius: 14px; border: 1px solid {GoldRadiant};'>
                    <div class='radiant-gold' style='font-size: 11px; font-weight: 800; color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px;'>
                        <span style='color: {GoldRadiant};'>{s.SelectedAddonsLabel}</span>
                    </div>
                    <ul class='radiant-body' style='margin: 0; padding-left: 18px; font-size: 13.5px; color: {TextIceWhite} !important; line-height: 1.6;'>");
            foreach (var addon in booking.SelectedAddons)
            {
                sb.Append($"<li style='margin-bottom: 4px;'><span style='color: {TextIceWhite};'>{addon.Title} — <strong style='color: {GoldRadiant};'>${addon.TotalPrice:N2}</strong></span></li>");
            }
            sb.Append("</ul></div>");
        }

        sb.Append($@"
                <!-- Next Steps Info Box -->
                <div style='background-color: {NavyTicket}; background-image: linear-gradient(#10263F, #0A1C30); border-left: 4px solid {GoldRadiant}; padding: 20px 22px; border-radius: 8px; margin-bottom: 24px;'>
                    <h4 class='radiant-gold' style='margin: 0 0 6px 0; font-size: 13px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 0.5px;'>
                        <span style='color: {GoldRadiant};'>{s.WhatNextTitle}</span>
                    </h4>
                    <p class='radiant-body' style='margin: 0; font-size: 13.5px; color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important; line-height: 1.6;'>
                        <span style='color: {TextIceWhite};'>{s.WhatNextDesc}</span>
                    </p>
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter(s));
        return sb.ToString();
    }

    public static string BuildConfirmationHtml(Domain.Entities.Booking booking)
    {
        var s = GetStrings(booking.Language);
        var refCode = booking.Id.ToString().Substring(0, 8).ToUpper();
        var tourDateFormatted = booking.TourDate.HasValue ? booking.TourDate.Value.ToString("dddd, MMMM dd, yyyy") : s.DateTbd;
        var pickupInfo = string.IsNullOrWhiteSpace(booking.PickupTime) ? "09:00 AM (Sharp)" : booking.PickupTime;
        var hotelInfo = string.IsNullOrWhiteSpace(booking.HotelName) ? "Private Luxury Vehicle Transfer" : $"{booking.HotelName} {(string.IsNullOrWhiteSpace(booking.RoomNumber) ? "" : $"(Room {booking.RoomNumber})")}";

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            s: s,
            preheader: s.PreheaderConfirmed,
            statusPill: s.StatusConfirmed,
            title: s.TitleConfirmed,
            isConfirmed: true
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20' style='padding: 38px 36px 28px 36px;'>
                <p style='margin: 0 0 6px 0; font-size: 12px; font-weight: 800; color: {GreenBright} !important; -webkit-text-fill-color: {GreenBright} !important; text-transform: uppercase; letter-spacing: 1.5px;'>
                    <span style='color: {GreenBright} !important;'>✓ {s.StatusConfirmed}</span>
                </p>
                <h2 class='radiant-white' style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; line-height: 1.3;'>
                    <span style='color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important;'>{s.Greeting} {booking.CustomerName},</span>
                </h2>
                <p class='radiant-body' style='margin: 0 0 28px 0; font-size: 14.5px; color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important; line-height: 1.75;'>
                    <span style='color: {TextIceWhite} !important; -webkit-text-fill-color: {TextIceWhite} !important;'>{s.ConfirmedIntro}</span>
                </p>

                <!-- Boarding Pass Style Voucher Card -->
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: {NavyTicket}; background-image: linear-gradient(180deg, #112C4C 0%, #0A1C30 100%); border: 2px solid {GoldRadiant}; border-radius: 16px; overflow: hidden; margin-bottom: 28px; box-shadow: 0 10px 30px rgba(0,0,0,0.5);'>
                    <tr>
                        <td style='background-color: #06152B; background-image: linear-gradient(145deg, #0D2644 0%, #06152B 100%); padding: 18px 22px; border-bottom: 2px solid {GoldRadiant};'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td>
                                        <span class='radiant-gold' style='font-size: 10px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1.5px;'>
                                            <span style='color: {GoldRadiant} !important;'>{s.OfficialVoucher}</span>
                                        </span>
                                        <div class='radiant-white' style='font-family: ""Playfair Display"", serif; font-size: 22px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; letter-spacing: 2px;'>
                                            <span style='color: #FFFFFF !important;'>#{refCode}</span>
                                        </div>
                                    </td>
                                    <td align='right'>
                                        <div style='display: inline-block; padding: 6px 18px; background-color: #059669; background-image: linear-gradient(135deg, #10B981 0%, #059669 100%); border: 1.5px solid #34D399; border-radius: 20px; font-size: 11px; font-weight: 800; color: #ffffff !important; -webkit-text-fill-color: #ffffff !important; letter-spacing: 1px; text-transform: uppercase;'>
                                            <span style='color: #ffffff !important;'>{s.StatusConfirmed}</span>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 24px 22px;'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td class='mobile-col' width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>{s.TourDate}</span></div>
                                        <div class='radiant-white' style='font-size: 15px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; margin-top: 4px;'><span style='color: #FFFFFF;'>{tourDateFormatted}</span></div>
                                    </td>
                                    <td class='mobile-col' width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>{s.PickupWindow}</span></div>
                                        <div class='radiant-white' style='font-size: 15px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; margin-top: 4px;'><span style='color: #FFFFFF;'>{pickupInfo}</span></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='mobile-col' width='50%' valign='top'>
                                        <div class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>{s.Guests}</span></div>
                                        <div class='radiant-white' style='font-size: 15px; font-weight: 800; color: #FFFFFF !important; -webkit-text-fill-color: #FFFFFF !important; margin-top: 4px;'><span style='color: #FFFFFF;'>{booking.Guests}</span></div>
                                    </td>
                                    <td class='mobile-col' width='50%' valign='top'>
                                        <div class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>{s.TotalAmount}</span></div>
                                        <div class='radiant-gold' style='font-size: 21px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; margin-top: 4px;'><span style='color: {GoldRadiant};'>${booking.TotalPrice:N2}</span></div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #081626; padding: 14px 22px; border-top: 1px dashed rgba(255,215,104,0.3); font-size: 13px; color: {TextIceWhite};'>
                            📍 <strong style='color: {GoldRadiant};'>{s.PickupLocation}:</strong> <span style='color: {TextIceWhite};'>{hotelInfo}</span>
                        </td>
                    </tr>
                </table>

                <!-- Registered Guests list if present -->
                {(booking.GuestsList != null && booking.GuestsList.Count > 0 ? $@"
                <div style='margin-bottom: 26px; padding: 18px 22px; background-color: {NavyTicket}; background-image: linear-gradient(#112C4C, #112C4C); border-radius: 14px; border: 1px solid {GoldRadiant};'>
                    <div class='radiant-gold' style='font-size: 11px; font-weight: 800; color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px;'>
                        <span style='color: {GoldRadiant};'>{s.GuestsBreakdownLabel}</span>
                    </div>
                    <ul class='radiant-body' style='margin: 0; padding-left: 18px; font-size: 13.5px; color: {TextIceWhite} !important; line-height: 1.6;'>
                        {string.Join("", booking.GuestsList.ConvertAll(g => $"<li style='margin-bottom: 4px;'><span style='color: {TextIceWhite};'>{g.FullName} ({g.Nationality ?? "VIP"})</span></li>"))}
                    </ul>
                </div>" : "")}

                <!-- VIP Travel Guidelines Box -->
                <div style='background-color: {NavyTicket}; background-image: linear-gradient(#10263F, #0A1C30); border-left: 4px solid {GoldRadiant}; padding: 20px 22px; border-radius: 8px; margin-bottom: 24px;'>
                    <h4 class='radiant-gold' style='margin: 0 0 8px 0; font-size: 13px; font-weight: 800; color: {GoldRadiant} !important; -webkit-text-fill-color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 0.5px;'>
                        <span style='color: {GoldRadiant};'>{s.RemindersTitle}</span>
                    </h4>
                    <ul class='radiant-body' style='margin: 0; padding-left: 18px; font-size: 13.5px; color: {TextIceWhite} !important; line-height: 1.65;'>
                        <li style='margin-bottom: 6px;'><span style='color: {TextIceWhite};'>{s.Reminder1}</span></li>
                        <li style='margin-bottom: 6px;'><span style='color: {TextIceWhite};'>{s.Reminder2}</span></li>
                        <li><span style='color: {TextIceWhite};'>{s.Reminder3}</span></li>
                    </ul>
                </div>

                <!-- Cancellation Guarantee -->
                <div style='background-color: #261B05; background-image: linear-gradient(135deg, #3D2B05 0%, #261B05 100%); border: 1.5px solid {GoldAmber}; padding: 16px 20px; border-radius: 10px; font-size: 12.5px; color: {GoldBright}; line-height: 1.55;'>
                    <strong style='color: {GoldRadiant};'>{s.FlexibilityTitle}:</strong> <span style='color: {GoldBright};'>{s.FlexibilityDesc}</span>
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter(s));
        return sb.ToString();
    }

    public static string BuildInquiryAutoReplyHtml(ContactInquiry inquiry)
    {
        var s = GetStrings("en");
        var destination = string.IsNullOrWhiteSpace(inquiry.DestinationInterest) ? "Egypt & The Red Sea" : inquiry.DestinationInterest;

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            s: s,
            preheader: $"Thank you for contacting Seadora Travel regarding {destination}.",
            statusPill: "Inquiry Received",
            title: "Thank You for Contacting Seadora Travel",
            isConfirmed: false
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20' style='padding: 38px 36px 28px 36px;'>
                <p class='radiant-gold' style='margin: 0 0 6px 0; font-size: 12px; font-weight: 800; color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1.5px;'>
                    <span style='color: {GoldRadiant};'>Inquiry Acknowledgment</span>
                </p>
                <h2 class='radiant-white' style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 800; color: #FFFFFF !important; line-height: 1.3;'>
                    <span style='color: #FFFFFF;'>Dear {inquiry.FullName},</span>
                </h2>
                <p class='radiant-body' style='margin: 0 0 20px 0; font-size: 14.5px; color: {TextIceWhite} !important; line-height: 1.75;'>
                    <span style='color: {TextIceWhite};'>Thank you for reaching out to <strong>Seadora Luxury Travel</strong>. We have received your inquiry regarding <strong>{destination}</strong>.</span>
                </p>

                <!-- Inquiry Summary Box -->
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: {NavyTicket}; background-image: linear-gradient(#112C4C, #112C4C); border: 1.5px solid {GoldRadiant}; border-radius: 14px; padding: 20px; margin-bottom: 24px;'>
                    <tr>
                        <td>
                            <div class='radiant-gold' style='font-size: 11px; font-weight: 800; color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px;'><span style='color: {GoldRadiant};'>Your Message / Request:</span></div>
                            <div class='radiant-body' style='font-size: 13.5px; color: {TextIceWhite} !important; font-style: italic; line-height: 1.6;'>
                                <span style='color: {TextIceWhite};'>""{inquiry.Message}""</span>
                            </div>
                            {(string.IsNullOrWhiteSpace(inquiry.DateOrGuests) ? "" : $"<div class='radiant-muted' style='margin-top: 10px; font-size: 12.5px; color: {TextMutedAzure};'><strong style='color: {GoldRadiant};'>Preferred Timing / Group:</strong> {inquiry.DateOrGuests}</div>")}
                        </td>
                    </tr>
                </table>

                <p class='radiant-body' style='margin: 0 0 20px 0; font-size: 14.5px; color: {TextIceWhite} !important; line-height: 1.75;'>
                    <span style='color: {TextIceWhite};'>One of our senior destination designers is preparing a personalized recommendation tailored to your schedule. We typically reply within 2 to 4 business hours.</span>
                </p>
            </td>
        </tr>");

        sb.Append(GetEmailFooter(s));
        return sb.ToString();
    }

    public static string BuildAdminInquiryReplyHtml(ContactInquiry inquiry, string replyMessage)
    {
        var s = GetStrings("en");
        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            s: s,
            preheader: $"Seadora Travel has replied to your travel inquiry.",
            statusPill: "Concierge Response",
            title: "Response to your Inquiry — Seadora Travel",
            isConfirmed: false
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20' style='padding: 38px 36px 28px 36px;'>
                <p class='radiant-gold' style='margin: 0 0 6px 0; font-size: 12px; font-weight: 800; color: {GoldRadiant} !important; text-transform: uppercase; letter-spacing: 1.5px;'>
                    <span style='color: {GoldRadiant};'>Seadora Concierge Team</span>
                </p>
                <h2 class='radiant-white' style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 800; color: #FFFFFF !important; line-height: 1.3;'>
                    <span style='color: #FFFFFF;'>Dear {inquiry.FullName},</span>
                </h2>
                <div class='radiant-body' style='margin: 0 0 24px 0; font-size: 14.5px; color: {TextIceWhite} !important; line-height: 1.75; background-color: {NavyTicket}; background-image: linear-gradient(#112C4C, #112C4C); border-left: 4px solid {GoldRadiant}; padding: 22px; border-radius: 10px;'>
                    <span style='color: {TextIceWhite};'>{replyMessage.Replace("\n", "<br/>")}</span>
                </div>

                <!-- Original Inquiry Reference -->
                <div style='border-top: 1px solid rgba(255,215,104,0.3); padding-top: 18px; margin-bottom: 20px;'>
                    <span class='radiant-muted' style='font-size: 11px; font-weight: 700; color: {TextMutedAzure} !important; text-transform: uppercase; letter-spacing: 0.5px;'><span style='color: {TextMutedAzure};'>Regarding your original message:</span></span>
                    <p class='radiant-muted' style='margin: 6px 0 0 0; font-size: 12.5px; color: {TextMutedAzure} !important; font-style: italic;'>
                        <span style='color: {TextMutedAzure};'>""{inquiry.Message}""</span>
                    </p>
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter(s));
        return sb.ToString();
    }
}
